using System;
using System.Diagnostics;
using System.Threading;

namespace Exomia.Framework.Buffers
{
    /// <summary>
    /// CircularBuffer class
    /// </summary>
    /// <typeparam name="T">any</typeparam>
    public class CircularBuffer<T>
    {
        private readonly T[] _buffer;

        private int _head;
        private int _tail;
        private int _size;

        private SpinLock _thisLock;

        /// <summary>
        /// Maximum capacity of the buffer.
        /// Elements pushed into the buffer after maximum capacity is reached will remove an element.
        /// </summary>
        public int Capacity { get { return _buffer.Length; } }

        /// <summary>
        /// <c>true</c> if the circular buffer is empty; <c>false</c> otherwise.
        /// </summary>
        public bool IsEmpty
        {
            get { return _size == 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _buffer[(_tail + index) % _buffer.Length];
            }
            set
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                _buffer[(_tail + index) % _buffer.Length] = value;
            }
        }


        /// <inheritdoc />
        /// <summary>
        ///  Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
        /// </summary>
        /// <param name="capacity">Buffer capacity. Must be positive.</param>
        public CircularBuffer(int capacity)
            : this(capacity, null) { }

        /// <summary>
        ///  Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
        /// </summary>
        /// <param name="capacity">Buffer capacity. Must be positive.</param>
        /// <param name="items">Items to fill buffer with. Items length must be less or equal than capacity.</param>
        public CircularBuffer(int capacity, T[] items)
        {
            if (capacity < 1)
            {
                throw new ArgumentException(@"Circular buffer cannot have negative or zero capacity.", nameof(capacity));
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
        /// 
        /// </summary>
        public void Clear()
        {
            _head = 0;
            _tail = 0;
            _size = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            bool lockTaken = false;
            try
            {
                _thisLock.Enter(ref lockTaken);
                if (_size == 0) throw new InvalidOperationException("empty circular buffer");

                T item = _buffer[_tail];
                _tail = (_tail + 1) % _buffer.Length;
                _size--;

                return item;
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ref T DequeueR()
        {
            bool lockTaken = false;
            try
            {
                _thisLock.Enter(ref lockTaken);
                if (_size == 0) throw new InvalidOperationException("empty circular buffer");

                ref T item = ref _buffer[_tail];
                _tail = (_tail + 1) % _buffer.Length;
                _size--;

                return ref item;
            }
            finally
            {
                if (lockTaken) { _thisLock.Exit(false); }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAdd"></param>
        public void Enqueue(in T toAdd)
        {
            bool lockTaken = false;
            try
            {
                _thisLock.Enter(ref lockTaken);

                _buffer[_head] = toAdd;
                _head = (_head + 1) % _buffer.Length;

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
