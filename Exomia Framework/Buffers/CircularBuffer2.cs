#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Exomia.Framework.Mathematics;

namespace Exomia.Framework.Buffers
{
    /// <summary>
    ///     CircularBuffer2 is an optimized version of <see cref="CircularBuffer {T}" /> for performance-critical use.
    /// </summary>
    /// <typeparam name="T">any</typeparam>
    public class CircularBuffer2<T>
    {
        private readonly T[] _buffer;

        private readonly int _mask;

        private int _head;
        private int _tail;
        private int _size;

        private SpinLock _thisLock;

        /// <summary>
        ///     Maximum capacity of the buffer.
        ///     Elements pushed into the buffer after maximum capacity is reached will remove an element.
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _buffer.Length; }
        }

        /// <summary>
        ///     <c>true</c> if the circular buffer is empty; <c>false</c> otherwise.
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _size == 0; }
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                index = (_tail + index) & _mask;
                T buffer = _buffer[index];
                _buffer[index] = default;
                return buffer;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                _buffer[(_tail + index) & _mask] = value;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBuffer{T}" /> class.
        /// </summary>
        /// <param name="capacity">Buffer capacity. Must be positive.</param>
        /// <param name="items">Items to fill buffer with. Items length must be less or equal than capacity.</param>
        public CircularBuffer2(int capacity = 1024, T[] items = null)
        {
            if (capacity < 1)
            {
                throw new ArgumentException(
                    @"Circular buffer cannot have zero capacity.", nameof(capacity));
            }
            capacity = Math2.RoundUpToPowerOfTwo(capacity);
            _mask = capacity - 1;

            _buffer = new T[capacity + 1];

            if (items != null)
            {
                if (items.Length > capacity)
                {
                    throw new ArgumentException(@"Too many items to fit circular buffer", nameof(items));
                }
                Array.Copy(items, _buffer, items.Length);
                _size = items.Length;
            }
            _tail = 0;
            _head = _size == capacity ? 0 : _size;

            _thisLock = new SpinLock(Debugger.IsAttached);
        }

        /// <summary>
        ///     clear the circular buffer
        /// </summary>
        public void Clear()
        {
            Array.Clear(_buffer, 0, _size);
            _head = 0;
            _tail = 0;
            _size = 0;
        }

        /// <summary>
        ///     get the last element and return its value
        /// </summary>
        /// <returns>value of the last element</returns>
        /// <exception cref="InvalidOperationException">if the buffer is empty</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get()
        {
            bool lockTaken = false;
            try
            {
                _thisLock.Enter(ref lockTaken);
                if (_size == 0)
                {
                    throw new InvalidOperationException("empty circular buffer");
                }

                int index = _tail++ & _mask;
                T item = _buffer[index];
                _buffer[index] = default;
                _size--;

                return item;
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        ///     get the last element and return its value
        /// </summary>
        /// <returns>value of the last element</returns>
        /// <exception cref="InvalidOperationException">if the buffer is empty</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetR()
        {
            bool lockTaken = false;
            try
            {
                _thisLock.Enter(ref lockTaken);
                if (_size == 0)
                {
                    throw new InvalidOperationException("empty circular buffer");
                }

                int index = _tail++ & _mask;
                ref T item = ref _buffer[index];
                _buffer[index] = default;
                _size--;

                return ref item;
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        ///     peek the last element and return its value
        ///     this method does not consume the element
        /// </summary>
        /// <returns>value of the last element</returns>
        /// <exception cref="InvalidOperationException">if the buffer is empty</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            bool lockTaken = false;
            try
            {
                _thisLock.Enter(ref lockTaken);
                if (_size == 0)
                {
                    throw new InvalidOperationException("empty circular buffer");
                }

                return _buffer[_tail & _mask];
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        ///     peek the last element and return its value
        ///     this method does not consume the element
        /// </summary>
        /// <returns>value of the last element</returns>
        /// <exception cref="InvalidOperationException">if the buffer is empty</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T PeekR()
        {
            bool lockTaken = false;
            try
            {
                _thisLock.Enter(ref lockTaken);
                if (_size == 0)
                {
                    throw new InvalidOperationException("empty circular buffer");
                }

                return ref _buffer[_tail & _mask];
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        ///     put an element to the start of the buffer
        /// </summary>
        /// <param name="toAdd">element to put at start</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Put(in T toAdd)
        {
            bool lockTaken = false;
            try
            {
                _thisLock.Enter(ref lockTaken);

                _buffer[_head++ & _mask] = toAdd;

                if (_size == _buffer.Length) { _tail++; }
                else { _size++; }
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }
    }
}