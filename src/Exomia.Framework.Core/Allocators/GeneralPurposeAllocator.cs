#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;
using System.Text;

namespace Exomia.Framework.Core.Allocators
{
    /// <summary> A general purpose allocator. This class cannot be inherited. </summary>
    public sealed unsafe class GeneralPurposeAllocator : IDisposable
    {
        private const int  DEFAULT_MAX_MEMORY_COUNT = 255;
        private const int  SIZE_PER_BUFFER          = 1 * 1024 * 1024 * 64; //1byte * 1024 = kib * 1024 = mib * 64 = 64mib
        private const uint NO_INDEX_BIT             = 0x80000000;
        private const uint SET_BIT                  = 0x40000000;
        private const uint INDEX_MASK               = 0x1FFFFFFF;

        private readonly uint     _maxBufferCount;
        private readonly Header** _buffers;
        private readonly Header** _freeBlock;
        private          uint     _bufferCount;

        private SpinLock _spinLock;

        /// <summary> Initializes a new instance of the <see cref="GeneralPurposeAllocator" /> class. </summary>
        /// <param name="maxBufferCount"> (Optional) Number of maximum buffers. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        public GeneralPurposeAllocator(uint maxBufferCount = DEFAULT_MAX_MEMORY_COUNT)
        {
            _maxBufferCount = maxBufferCount <= INDEX_MASK
                ? maxBufferCount
                : throw new ArgumentOutOfRangeException(nameof(maxBufferCount));
            _buffers   = (Header**)Marshal.AllocHGlobal((int)maxBufferCount);
            _freeBlock = (Header**)Marshal.AllocHGlobal((int)maxBufferCount);
            GC.AddMemoryPressure(maxBufferCount << 1);
        }

        /// <summary> Allocates. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <returns> Null if it fails, else a byte*. </returns>
        public T Allocate<T>() where T : unmanaged
        {
            return *(T*)Allocate(sizeof(T));
        }

        /// <summary> Allocate a null terminated string. </summary>
        /// <param name="value"> The utf-16 string value. </param>
        /// <returns> Null if it fails, else a byte*. </returns>
        public sbyte* AllocateNtString(string value)
        {
            int   maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length) + 1;
            byte* buffer       = Allocate(maxByteCount);

            fixed (char* srcPointer = value)
            {
                buffer[Encoding.UTF8.GetBytes(srcPointer, value.Length, buffer, maxByteCount)] = 0;
            }

            return (sbyte*)buffer;
        }

        /// <summary> Allocates a specified size of bytes. </summary>
        /// <param name="sizeInBytes"> The size in bytes. </param>
        /// <returns> Null if it fails, else a byte*. </returns>
        public byte* Allocate(int sizeInBytes)
        {
            if (sizeInBytes + Header.SIZE >= SIZE_PER_BUFFER)
            {
                byte* h = (byte*)Marshal.AllocHGlobal(sizeInBytes + Header.SIZE);
                GC.AddMemoryPressure(sizeInBytes + Header.SIZE);

                Header* buffer = (Header*)h;
                buffer->BufferIndex   = NO_INDEX_BIT;
                buffer->PreviousBlock = null;
                buffer->NextFreeBlock = null;
                buffer->Length        = sizeInBytes;

                return h + Header.SIZE;
            }

            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                return (byte*)FindFreeBuffer(sizeInBytes) + Header.SIZE;
            }
            finally
            {
                if (lockTaken) { _spinLock.Exit(false); }
            }
        }

        /// <summary> Frees the given pointer. </summary>
        /// <param name="ptr"> [in,out] [in,out] If non-null, the pointer. </param>
        /// <remarks> Must be allocated with this instance of allocator. </remarks>
        public void Free(ref byte* ptr)
        {
            Header* header = (Header*)(ptr - Header.SIZE);
            if ((header->BufferIndex & NO_INDEX_BIT) == NO_INDEX_BIT)
            {
                Marshal.FreeHGlobal((IntPtr)header);
                GC.RemoveMemoryPressure(header->Length);
                return;
            }

            uint bufferIndex = header->BufferIndex & INDEX_MASK;

            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);

                Header* previous = header->PreviousBlock;
                Header* next     = (Header*)((byte*)header + header->Length);

                bool dPrevious = previous != null && (previous->BufferIndex & SET_BIT) == 0;
                bool dNext     = /*next   != null && */(next->BufferIndex & SET_BIT) == 0;

                if (dPrevious && dNext)
                {
                    previous->Length += header->Length + next->Length;

                    ((Header*)((byte*)next + next->Length))->PreviousBlock = previous;

                    Header* buffer = *(_freeBlock + bufferIndex);
                    if (next == buffer)
                    {
                        *(_freeBlock + bufferIndex) = next->NextFreeBlock;
                    }
                    else
                    {
                        while (buffer->NextFreeBlock != next)
                        {
                            buffer = buffer->NextFreeBlock;
                        }
                        buffer->NextFreeBlock = next->NextFreeBlock;
                    }
                }
                else if (dNext)
                {
                    header->BufferIndex   =  INDEX_MASK;
                    header->NextFreeBlock =  next->NextFreeBlock;
                    header->Length        += next->Length;

                    ((Header*)((byte*)next + next->Length))->PreviousBlock = header;

                    Header* buffer = *(_freeBlock + bufferIndex);
                    if (next == buffer)
                    {
                        *(_freeBlock + bufferIndex) = header;
                    }
                    else
                    {
                        while (buffer->NextFreeBlock != next)
                        {
                            buffer = buffer->NextFreeBlock;
                        }
                        buffer->NextFreeBlock = header;
                    }
                }
                else if (dPrevious)
                {
                    previous->Length    += header->Length;
                    next->PreviousBlock =  previous;
                }
                else
                {
                    header->BufferIndex   = INDEX_MASK;
                    header->NextFreeBlock = *(_freeBlock + bufferIndex);

                    *(_freeBlock + bufferIndex) = header;
                }
            }
            finally
            {
                if (lockTaken) { _spinLock.Exit(false); }
            }

            ptr = null;
        }

        /// <summary> Resets all buffers. </summary>
        public void Reset()
        {
            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                _bufferCount = 0;
            }
            finally
            {
                if (lockTaken) { _spinLock.Exit(false); }
            }
        }

        private Header* FindFreeBuffer(int sizeInBytes)
        {
            int offset = Header.SIZE + sizeInBytes;
            if ((offset & 3) != 0)
            {
                offset += 4 - (offset & 3);
            }

            for (uint bufferIndex = 0; bufferIndex < _bufferCount; bufferIndex++)
            {
                Header* buffer = *(_freeBlock + bufferIndex);
                if (buffer == null) { continue; }
                if (buffer->Length >= offset)
                {
                    if (buffer->Length >= offset + Header.SIZEx2)
                    {
                        Header* nextFreeBlock = *(_freeBlock + bufferIndex) = (Header*)((byte*)buffer + offset);
                        nextFreeBlock->BufferIndex   = INDEX_MASK;
                        nextFreeBlock->PreviousBlock = buffer;
                        nextFreeBlock->NextFreeBlock = buffer->NextFreeBlock;
                        nextFreeBlock->Length        = buffer->Length - offset;

                        ((Header*)((byte*)buffer + buffer->Length))->PreviousBlock = nextFreeBlock;

                        buffer->BufferIndex   = SET_BIT | bufferIndex;
                        buffer->PreviousBlock = buffer->PreviousBlock;
                        buffer->NextFreeBlock = null;
                        buffer->Length        = offset;
                    }
                    else
                    {
                        *(_freeBlock + bufferIndex) = buffer->NextFreeBlock;

                        buffer->BufferIndex   = SET_BIT | bufferIndex;
                        buffer->PreviousBlock = buffer->PreviousBlock;
                        buffer->NextFreeBlock = null;
                        buffer->Length        = buffer->Length;
                    }

                    return buffer;
                }

                Header* previous = buffer;
                do
                {
                    buffer = buffer->NextFreeBlock;
                    if (buffer != null && buffer->Length >= offset)
                    {
                        if (buffer->Length >= offset + Header.SIZEx2)
                        {
                            Header* nextFreeBlock = previous->NextFreeBlock = (Header*)((byte*)buffer + offset);
                            nextFreeBlock->BufferIndex   = INDEX_MASK;
                            nextFreeBlock->PreviousBlock = buffer;
                            nextFreeBlock->NextFreeBlock = buffer->NextFreeBlock;
                            nextFreeBlock->Length        = buffer->Length - offset;

                            ((Header*)((byte*)buffer + buffer->Length))->PreviousBlock = nextFreeBlock;

                            buffer->BufferIndex   = SET_BIT | bufferIndex;
                            buffer->PreviousBlock = buffer->PreviousBlock;
                            buffer->NextFreeBlock = null;
                            buffer->Length        = offset;
                        }
                        else
                        {
                            previous->NextFreeBlock = buffer->NextFreeBlock;

                            buffer->BufferIndex   = SET_BIT | bufferIndex;
                            buffer->PreviousBlock = buffer->PreviousBlock;
                            buffer->NextFreeBlock = null;
                            buffer->Length        = buffer->Length;

                            return buffer;
                        }

                        return buffer;
                    }

                    previous = buffer;
                }
                while (buffer != null);
            }

            return NewBuffer(offset);
        }

        private Header* NewBuffer(int sizeInBytes)
        {
            uint bufferIndex = _bufferCount++;

            Header* buffer = *(_buffers + bufferIndex) = (Header*)Marshal.AllocHGlobal(SIZE_PER_BUFFER);
            GC.AddMemoryPressure(SIZE_PER_BUFFER);
            buffer->BufferIndex   = SET_BIT | bufferIndex;
            buffer->PreviousBlock = null;
            buffer->NextFreeBlock = null;
            buffer->Length        = sizeInBytes;

            Header* nextFreeBlock = *(_freeBlock + bufferIndex) = (Header*)((byte*)buffer + sizeInBytes);
            nextFreeBlock->BufferIndex   = INDEX_MASK;
            nextFreeBlock->PreviousBlock = buffer;
            nextFreeBlock->NextFreeBlock = null;
            nextFreeBlock->Length        = SIZE_PER_BUFFER - sizeInBytes - Header.SIZE;

            Header* endBlock = (Header*)((byte*)buffer + (SIZE_PER_BUFFER - Header.SIZE));
            endBlock->BufferIndex   = SET_BIT | INDEX_MASK; //make sure we can't merge with this endBlock
            endBlock->PreviousBlock = null;
            endBlock->NextFreeBlock = null;
            endBlock->Length        = 0;

            return buffer;
        }

        [StructLayout(LayoutKind.Explicit, Size = SIZE)]
        private struct Header
        {
            /// <summary> The size. </summary>
            public const int SIZE = 6 * sizeof(int);

            // ReSharper disable once InconsistentNaming
            public const int SIZEx2 = SIZE << 1;

            private const int BUFFER_INDEX_OFFSET    = 0;
            private const int PREVIOUS_BLOCK_OFFSET  = 4;
            private const int NEXT_FREE_BLOCK_OFFSET = 12;
            private const int LENGTH_OFFSET          = 20;

            /// <summary> Zero-based index of the buffer. </summary>
            [FieldOffset(BUFFER_INDEX_OFFSET)]
            public uint BufferIndex;

            /// <summary> The previous block. </summary>
            [FieldOffset(PREVIOUS_BLOCK_OFFSET)]
            public Header* PreviousBlock;

            /// <summary> The next free block. </summary>
            [FieldOffset(NEXT_FREE_BLOCK_OFFSET)]
            public Header* NextFreeBlock;

            /// <summary> The length. </summary>
            [FieldOffset(LENGTH_OFFSET)]
            public int Length;
        }

        #region IDisposable Support

        private bool _disposed;

        /// <summary> Finalizes an instance of the <see cref="GeneralPurposeAllocator" /> class. </summary>
        ~GeneralPurposeAllocator()
        {
            Dispose();
        }

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                for (int i = 0; i < _bufferCount; i++)
                {
                    Marshal.FreeHGlobal((IntPtr)(*(_buffers + i)));
                }

                Marshal.FreeHGlobal((IntPtr)_freeBlock);
                Marshal.FreeHGlobal((IntPtr)_buffers);
                GC.RemoveMemoryPressure((SIZE_PER_BUFFER * _bufferCount) + (_maxBufferCount << 1));

                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}