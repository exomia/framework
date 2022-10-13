#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Core.Graphics;

public partial class Canvas
{
    sealed unsafe class ItemBuffer : IDisposable
    {
        private Item*    _buffer;
        private uint     _length;
        private uint     _count;
        private uint     _rectanglesOffset;
        private SpinLock _spinLock = new SpinLock(Debugger.IsAttached);

        /// <summary> Gets the current count of items reserved. </summary>
        /// <value> The current amount of reserved items. </value>
        public uint Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _count; }
        }
        
        /// <summary> Gets the current count of rectangles required. </summary>
        /// <value> The current amount of rectangles required. </value>
        public uint RectangleCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _rectanglesOffset; }
        }

        /// <summary> Initializes a new instance of the <see cref="ItemBuffer" /> class. </summary>
        /// <param name="initialCount"> The initial count of items the buffer can hold without resizing. </param>
        public ItemBuffer(uint initialCount)
        {
            _buffer          = Allocator.Allocate<Item>(_length = initialCount);
            _count           = 0;
            _rectanglesOffset = 0;
        }

        /// <summary> Implicit cast that converts the given <see cref="ItemBuffer" /> to a <see name="Item" />*. </summary>
        /// <param name="buffer"> The structure buffer. </param>
        /// <returns> The result of the operation. </returns>
        public static implicit operator Item*(ItemBuffer buffer)
        {
            return buffer._buffer;
        }

        /// <summary> Reserves one <see cref="Item"/>. </summary>
        /// <param name="rectangleCount"> The amount of rectangles to reserve. </param>
        /// <returns> A pointer to the reserved item. </returns>
        public Item* Reserve(uint rectangleCount)
        {
            uint rectangleStartOffset = Interlocked.Add(ref _rectanglesOffset, rectangleCount) - rectangleCount;

            uint index = Interlocked.Increment(ref _count);
            if (index >= _length)
            {
                bool lockTaken = false;
                try
                {
                    _spinLock.Enter(ref lockTaken);
                    if (index >= _length)
                    {
                        Allocator.Resize(ref _buffer, ref _length, _length * 2);
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _spinLock.Exit(false);
                    }
                }
            }

            Item* item = _buffer + (index - 1u);
            item->RectangleStartOffset = rectangleStartOffset;
            return item;
        }

        /// <summary> Resets the buffer count back to zero </summary>
        public void Reset()
        {
            Interlocked.Exchange(ref _rectanglesOffset, 0);
            Interlocked.Exchange(ref _count,           0);
        }

        #region IDisposable Support

        private bool _disposed;

        private void ReleaseUnmanagedResources()
        {
            if (!_disposed)
            {
                Allocator.Free(ref _buffer, _length);

                _disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        ~ItemBuffer()
        {
            ReleaseUnmanagedResources();
        }

        #endregion
    }
}