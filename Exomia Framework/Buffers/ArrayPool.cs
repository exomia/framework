#pragma warning disable CS1591

using System;
using System.Diagnostics;
using System.Threading;

namespace Exomia.Framework.Buffers
{
    public sealed class ArrayPool<T>
    {
        #region Constructors

        #region Statics

        #endregion

        public ArrayPool(int bufferLength, int numberOfBuffers = 10)
        {
            if (bufferLength <= 0) { throw new ArgumentOutOfRangeException(nameof(bufferLength)); }
            if (numberOfBuffers <= 0) { throw new ArgumentOutOfRangeException(nameof(numberOfBuffers)); }

            _bufferLength = bufferLength;
            _lock = new SpinLock(Debugger.IsAttached);
            _buffers = new T[numberOfBuffers][];
        }

        #endregion

        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        private SpinLock _lock;
        private readonly T[][] _buffers;
        private int _index;

        private readonly int _bufferLength;

        #endregion

        #region Properties

        #region Statics

        #endregion

        #endregion

        #region Methods

        #region Statics

        #endregion

        public T[] Rent()
        {
            T[] buffer = null;

            bool lockTaken = false, allocateBuffer = false;
            try
            {
                _lock.Enter(ref lockTaken);

                if (_index < _buffers.Length)
                {
                    buffer = _buffers[_index];
                    _buffers[_index++] = null;
                    allocateBuffer = buffer == null;
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _lock.Exit(false);
                }
            }

            return !allocateBuffer ? buffer : new T[_bufferLength];
        }

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

        #endregion
    }
}