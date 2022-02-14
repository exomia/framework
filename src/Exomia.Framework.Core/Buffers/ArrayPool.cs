#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;

namespace Exomia.Framework.Core.Buffers
{
    /// <summary> ArrayPool class. </summary>
    /// <typeparam name="T"> any. </typeparam>
    public sealed class ArrayPool<T>
    {
        private readonly int      _bufferLength;
        private readonly T[]?[]   _buffers;
        private          SpinLock _lock;
        private          int      _index;

        /// <summary> Initializes a new instance of the <see cref="ArrayPool{T}" /> class. </summary>
        /// <param name="bufferLength">    Length of the buffer. </param>
        /// <param name="numberOfBuffers"> (Optional) Number of buffers. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        public ArrayPool(int bufferLength, int numberOfBuffers = 10)
        {
            if (bufferLength <= 0) { throw new ArgumentOutOfRangeException(nameof(bufferLength)); }
            if (numberOfBuffers <= 0) { throw new ArgumentOutOfRangeException(nameof(numberOfBuffers)); }

            _bufferLength = bufferLength;
            _lock         = new SpinLock(Debugger.IsAttached);
            _buffers      = new T[numberOfBuffers][];
        }

        /// <summary> Gets the rent. </summary>
        /// <returns> A T[]. </returns>
        public T[] Rent()
        {
            T[]? buffer = null;

            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);

                if (_index < _buffers.Length)
                {
                    buffer             = _buffers[_index];
                    _buffers[_index++] = null;
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _lock.Exit(false);
                }
            }

            return buffer ?? new T[_bufferLength];
        }

        /// <summary> Returns. </summary>
        /// <param name="array">      The array. </param>
        /// <param name="clearArray"> True to clear array. </param>
        /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
        public void Return(T[] array, bool clearArray)
        {
            if (array.Length != _bufferLength)
            {
                throw new ArgumentException(nameof(array));
            }

            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);

                if (_index != 0)
                {
                    if (clearArray) { Array.Clear(array, 0, array.Length); }
                    _buffers[--_index] = array;
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _lock.Exit(false);
                }
            }
        }
    }
}