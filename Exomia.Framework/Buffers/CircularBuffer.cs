#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Exomia.Framework.Buffers
{
    /// <summary>
    ///     CircularBuffer class.
    /// </summary>
    /// <typeparam name="T"> any. </typeparam>
    public class CircularBuffer<T>
    {
        /// <summary>
        ///     The buffer.
        /// </summary>
        private readonly T[] _buffer;

        /// <summary>
        ///     this lock.
        /// </summary>
        private readonly SpinLock _thisLock;

        /// <summary>
        ///     The head.
        /// </summary>
        private int _head;

        /// <summary>
        ///     The size.
        /// </summary>
        private int _size;

        /// <summary>
        ///     The tail.
        /// </summary>
        private int _tail;

        /// <summary>
        ///     Maximum capacity of the buffer. Elements pushed into the buffer after maximum capacity is
        ///     reached will remove an element.
        /// </summary>
        /// <value>
        ///     The capacity.
        /// </value>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _buffer.Length; }
        }

        /// <summary>
        ///     <c>true</c> if the circular buffer is empty; <c>false</c> otherwise.
        /// </summary>
        /// <value>
        ///     True if this object is empty, false if not.
        /// </value>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _size == 0; }
        }

        /// <summary>
        ///     Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="index"> . </param>
        /// <returns>
        ///     The indexed item.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when one or more arguments are outside
        ///     the required range.
        /// </exception>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                index = (_tail + index) % _buffer.Length;
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
                _buffer[(_tail + index) % _buffer.Length] = value;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CircularBuffer{T}" /> class.
        /// </summary>
        /// <param name="capacity"> (Optional) Buffer capacity. Must be positive. </param>
        /// <param name="items">
        ///     (Optional) Items to fill buffer with. Items length must be less or
        ///     equal than capacity.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        public CircularBuffer(int capacity = 1024, T[] items = null)
        {
            if (capacity < 1)
            {
                throw new ArgumentException(
                    @"Circular buffer cannot have negative or zero capacity.", nameof(capacity));
            }
            _buffer = new T[capacity];

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
        ///     clear the circular buffer.
        /// </summary>
        public void Clear()
        {
            Array.Clear(_buffer, 0, _size);
            _head = 0;
            _tail = 0;
            _size = 0;
        }

        /// <summary>
        ///     get the last element and return its value.
        /// </summary>
        /// <returns>
        ///     value of the last element.
        /// </returns>
        /// <exception cref="InvalidOperationException"> if the buffer is empty. </exception>
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

                T item = _buffer[_tail];
                _buffer[_tail] = default;
                _tail          = (_tail + 1) % _buffer.Length;
                _size--;

                return item;
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        ///     get the last element from the buffer and return its value.
        /// </summary>
        /// <returns>
        ///     value of the last element.
        /// </returns>
        /// <exception cref="InvalidOperationException"> if the buffer is empty. </exception>
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

                ref T item = ref _buffer[_tail];
                _buffer[_tail] = default;
                _tail          = (_tail + 1) % _buffer.Length;
                _size--;

                return ref item;
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        ///     peek the last element and return its value this method does not consume the element.
        /// </summary>
        /// <returns>
        ///     value of the last element.
        /// </returns>
        /// <exception cref="InvalidOperationException"> if the buffer is empty. </exception>
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

                return _buffer[_tail];
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        ///     peek the last element and return its value this method does not consume the element.
        /// </summary>
        /// <returns>
        ///     value of the last element.
        /// </returns>
        /// <exception cref="InvalidOperationException"> if the buffer is empty. </exception>
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

                return ref _buffer[_tail];
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        ///     put an element to the start of the buffer.
        /// </summary>
        /// <param name="toAdd"> element to put at start. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Put(in T toAdd)
        {
            bool lockTaken = false;
            try
            {
                _thisLock.Enter(ref lockTaken);

                _buffer[_head] = toAdd;
                _head          = (_head + 1) % _buffer.Length;

                if (_size == _buffer.Length)
                {
                    _tail = (_tail + 1) % _buffer.Length;
                }
                else { _size++; }
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }
    }
}