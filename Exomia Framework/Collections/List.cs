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

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Collections
{
    public sealed class List<T>
    {
        #region Variables

        private const int DEFAULT_CAPACITY = 8;
        private const int MAX_CAPACITY = 0X7FEFFFFF;

        private static readonly T[] s_emptyArray = new T[0];

        private readonly int _sizeOf;

        private T[] _items;

        #endregion

        #region Properties

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _items.Length; }
            set
            {
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(Capacity)} must be greater than the current size.");
                }
                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (Count > 0) { Buffer.BlockCopy(_items, 0, newItems, 0, Count * _sizeOf); }
                        _items = newItems;
                    }
                    else { _items = s_emptyArray; }
                }
            }
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _items[index]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _items[index] = value; }
        }

        public T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _items[index]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _items[index] = value; }
        }

        #endregion

        #region Constructors

        public List()
        {
            _items = s_emptyArray;
            _sizeOf = Marshal.SizeOf<T>();
        }

        public List(uint capacity)
        {
            _items = capacity == 0 ? s_emptyArray : new T[capacity];
            _sizeOf = Marshal.SizeOf<T>();
        }

        public List(int capacity)
        {
            if (capacity < 0) { throw new ArgumentOutOfRangeException(nameof(capacity)); }
            _items = capacity == 0 ? s_emptyArray : new T[capacity];
            _sizeOf = Marshal.SizeOf<T>();
        }

        public List(IEnumerable<T> collection)
        {
            if (collection == null) { throw new ArgumentNullException(nameof(collection)); }
            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count == 0) { _items = s_emptyArray; }
                else
                {
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    Count = count;
                }
            }
            else
            {
                Count = 0;
                _items = s_emptyArray;

                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Add(en.Current);
                    }
                }
            }
        }

        #endregion

        #region Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int index)
        {
            return ref _items[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(uint index)
        {
            return ref _items[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T item)
        {
            if (Count == _items.Length) { EnsureCapacity(Count + 1); }
            _items[Count++] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, in T item)
        {
            if (Count == _items.Length) { EnsureCapacity(Count + 1); }
            if (index < Count) { Buffer.BlockCopy(_items, index, _items, index + 1, (Count - index) * _sizeOf); }
            _items[index] = item;
            Count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(in T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            Count--;
            if (index < Count) { Buffer.BlockCopy(_items, index + 1, _items, index, (Count - index) * _sizeOf); }
            _items[Count] = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRange(int index, int count)
        {
            if (count > 0)
            {
                Count -= count;
                if (index < Count)
                {
                    Buffer.BlockCopy(_items, index + count, _items, index, (Count - index) * _sizeOf);
                }
                Array.Clear(_items, Count, count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(_items, 0, Count);
                Count = 0;
            }
        }

        public bool Contains(in T item)
        {
            if (item == null)
            {
                for (int i = 0; i < Count; ++i)
                {
                    if (_items[i] == null) { return true; }
                }
                return false;
            }
            for (int i = 0; i < Count; ++i)
            {
                if (_items[i].Equals(item)) { return true; }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> GetRange(int index, int count)
        {
            List<T> list = new List<T>(count);
            Buffer.BlockCopy(_items, index, list._items, 0, count * _sizeOf);
            list.Count = count;
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item)
        {
            return Array.IndexOf(_items, item, 0, Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item, int index)
        {
            return Array.IndexOf(_items, item, index, Count - index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item, int index, int count)
        {
            return Array.IndexOf(_items, item, index, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(in T item)
        {
            return Array.LastIndexOf(_items, item, 0, Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(in T item, int index)
        {
            return Array.LastIndexOf(_items, item, index, Count - index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(in T item, int index, int count)
        {
            return Array.LastIndexOf(_items, item, index, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse()
        {
            Array.Reverse(_items, 0, Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse(int index, int count)
        {
            Array.Reverse(_items, index, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort()
        {
            Sort(0, Count, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            Array.Sort(_items, index, count, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            T[] array = new T[Count];
            Buffer.BlockCopy(_items, 0, array, 0, Count * _sizeOf);
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray(int index)
        {
            T[] array = new T[Count - index];
            Buffer.BlockCopy(_items, index, array, 0, (Count - index) * _sizeOf);
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray(int index, int count)
        {
            T[] array = new T[count];
            Buffer.BlockCopy(_items, index, array, 0, count * _sizeOf);
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = _items.Length == 0 ? DEFAULT_CAPACITY : _items.Length * 2;
                if (newCapacity > MAX_CAPACITY) { newCapacity = MAX_CAPACITY; }
                if (newCapacity < min) { newCapacity = min; }

                T[] newItems = new T[newCapacity];
                if (Count > 0) { Buffer.BlockCopy(_items, 0, newItems, 0, Count * _sizeOf); }
                _items = newItems;
            }
        }

        #endregion
    }
}