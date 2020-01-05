#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Collections
{
    /// <summary>
    ///     List class.
    /// </summary>
    /// <typeparam name="T"> any. </typeparam>
    public sealed class List<T>
    {
        /// <summary>
        ///     The default capacity.
        /// </summary>
        private const int DEFAULT_CAPACITY = 8;

        /// <summary>
        ///     The maximum capacity.
        /// </summary>
        private const int MAX_CAPACITY = 0X7FEFFFFF;

        /// <summary>
        ///     Array of empties.
        /// </summary>
        private static readonly T[] s_emptyArray = new T[0];

        /// <summary>
        ///     The size of T in bytes.
        /// </summary>
        private readonly int _sizeOf;

        /// <summary>
        ///     The items.
        /// </summary>
        private T[] _items;

        /// <summary>
        ///     Sets the capacity.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        /// <value>
        ///     The capacity.
        /// </value>
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

        /// <summary>
        ///     Gets the number of items in the list.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }

        /// <summary>
        ///     Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="index"> Zero-based index of the entry to access. </param>
        /// <returns>
        ///     The indexed item.
        /// </returns>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _items[index]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _items[index] = value; }
        }

        /// <summary>
        ///     Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="index"> Zero-based index of the entry to access. </param>
        /// <returns>
        ///     The indexed item.
        /// </returns>
        public T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _items[index]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _items[index] = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the &lt;see cref="List&lt;T&gt;"/&gt; class.
        /// </summary>
        public List()
        {
            _items  = s_emptyArray;
            _sizeOf = Marshal.SizeOf<T>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="List{T}" /> class.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        public List(uint capacity)
        {
            _items  = capacity == 0 ? s_emptyArray : new T[capacity];
            _sizeOf = Marshal.SizeOf<T>();
        }

        /// <summary>
        ///     Initializes a new instance of the &lt;see cref="List&lt;T&gt;"/&gt; class.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when one or more arguments are outside
        ///     the required range.
        /// </exception>
        public List(int capacity)
        {
            if (capacity < 0) { throw new ArgumentOutOfRangeException(nameof(capacity)); }
            _items  = capacity == 0 ? s_emptyArray : new T[capacity];
            _sizeOf = Marshal.SizeOf<T>();
        }

        /// <summary>
        ///     Initializes a new instance of the &lt;see cref="List&lt;T&gt;"/&gt; class.
        /// </summary>
        /// <param name="collection"> The collection. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public List(IEnumerable<T> collection)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
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
                Count  = 0;
                _items = s_emptyArray;

                using IEnumerator<T> en = collection.GetEnumerator();
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }
        }

        /// <summary>
        ///     Adds the item to the end of the list.
        /// </summary>
        /// <param name="item"> The item to add to the end of the list. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T item)
        {
            Insert(Count, item);
        }

        /// <summary>
        ///     Adds a range of items to the end of the list.
        /// </summary>
        /// <param name="items"> The items to add to the end of the list. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(T[] items)
        {
            InsertRange(Count, items);
        }

        /// <summary>
        ///     Clears this object to its blank/initial state.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(_items, 0, Count);
                Count = 0;
            }
        }

        /// <summary>
        ///     Query if this object contains the given item.
        /// </summary>
        /// <param name="item"> The item to look for. </param>
        /// <returns>
        ///     True if the object is in this collection, false if not.
        /// </returns>
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

        /// <summary>
        ///     Gets a reference t using the given index.
        /// </summary>
        /// <param name="index"> Zero-based index of the item to get. </param>
        /// <returns>
        ///     A ref T.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int index)
        {
            return ref _items[index];
        }

        /// <summary>
        ///     Gets a reference t using the given index.
        /// </summary>
        /// <param name="index"> Zero-based index of the item to get. </param>
        /// <returns>
        ///     A ref T.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(uint index)
        {
            return ref _items[index];
        }

        /// <summary>
        ///     Finds the range of the given arguments.
        /// </summary>
        /// <param name="index"> Zero-based index to start copying from. </param>
        /// <param name="count"> Number of. </param>
        /// <returns>
        ///     The calculated range.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> GetRange(int index, int count)
        {
            List<T> list = new List<T>(count);
            Buffer.BlockCopy(_items, index, list._items, 0, count * _sizeOf);
            list.Count = count;
            return list;
        }

        /// <summary>
        ///     Searches for the first match.
        /// </summary>
        /// <param name="item"> The item to find the index of. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item)
        {
            return Array.IndexOf(_items, item, 0, Count);
        }

        /// <summary>
        ///     Searches for the first match.
        /// </summary>
        /// <param name="item">  The item to find the index of. </param>
        /// <param name="index"> Zero-based index to start searching from. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item, int index)
        {
            return Array.IndexOf(_items, item, index, Count - index);
        }

        /// <summary>
        ///     Searches for the first match.
        /// </summary>
        /// <param name="item">  The item to find the index of. </param>
        /// <param name="index"> Zero-based index to start searching from. </param>
        /// <param name="count"> Number of the search range. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item, int index, int count)
        {
            return Array.IndexOf(_items, item, index, count);
        }

        /// <summary>
        ///     Inserts the item into the list.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <param name="item">  The item to insert at index. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, in T item)
        {
            if (index >= _items.Length) { EnsureCapacity(Count + 1); }
            if (index < Count) { Array.Copy(_items, index, _items, index + 1, Count - index); }
            _items[index] =  item;
            Count         += 1;
        }

        /// <summary>
        ///     Inserts the range of items into the list.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <param name="items"> The items to insert at index. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertRange(int index, T[] items)
        {
            if (index + items.Length >= _items.Length) { EnsureCapacity(Count + items.Length); }
            if (index < Count) { Array.Copy(_items, index, _items, index + _items.Length, Count - index); }
            Buffer.BlockCopy(items, 0, _items, index, items.Length * _sizeOf);
            Count += _items.Length;
        }

        /// <summary>
        ///     Searches for the last match.
        /// </summary>
        /// <param name="item"> The item to find the last index of. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(in T item)
        {
            return Array.LastIndexOf(_items, item, 0, Count);
        }

        /// <summary>
        ///     Searches for the last match.
        /// </summary>
        /// <param name="item">  The item to find the last index of. </param>
        /// <param name="index"> Zero-based index to start searching from. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(in T item, int index)
        {
            return Array.LastIndexOf(_items, item, index, Count - index);
        }

        /// <summary>
        ///     Searches for the last match.
        /// </summary>
        /// <param name="item">  The item to find the last index of. </param>
        /// <param name="index"> Zero-based index to start searching from. </param>
        /// <param name="count"> Number of the search range. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(in T item, int index, int count)
        {
            return Array.LastIndexOf(_items, item, index, count);
        }

        /// <summary>
        ///     Removes the given item.
        /// </summary>
        /// <param name="item"> The item to remove. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
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

        /// <summary>
        ///     Removes at described by index.
        /// </summary>
        /// <param name="index"> Zero-based index of the item to remove. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            Count--;
            if (index < Count) { Array.Copy(_items, index + 1, _items, index, Count - index); }
            _items[Count] = default;
        }

        /// <summary>
        ///     Removes the range.
        /// </summary>
        /// <param name="index"> Zero-based index of the items to remove. </param>
        /// <param name="count"> Number of items to remove. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRange(int index, int count)
        {
            if (count > 0)
            {
                Count -= count;
                if (index < Count) { Array.Copy(_items, index + count, _items, index, Count - index); }
                Array.Clear(_items, Count, count);
            }
        }

        /// <summary>
        ///     Reverses.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse()
        {
            Array.Reverse(_items, 0, Count);
        }

        /// <summary>
        ///     Reverses.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <param name="count"> Number of. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse(int index, int count)
        {
            Array.Reverse(_items, index, count);
        }

        /// <summary>
        ///     Sorts.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort()
        {
            Sort(0, Count, null);
        }

        /// <summary>
        ///     Sorts.
        /// </summary>
        /// <param name="comparer"> The comparer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        /// <summary>
        ///     Sorts.
        /// </summary>
        /// <param name="index">    Zero-based index of the. </param>
        /// <param name="count">    Number of. </param>
        /// <param name="comparer"> The comparer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            Array.Sort(_items, index, count, comparer);
        }

        /// <summary>
        ///     Convert this object into an array representation.
        /// </summary>
        /// <returns>
        ///     An array that represents the data in this object.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            T[] array = new T[Count];
            Buffer.BlockCopy(_items, 0, array, 0, Count * _sizeOf);
            return array;
        }

        /// <summary>
        ///     Convert this object into an array representation.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <returns>
        ///     An array that represents the data in this object.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray(int index)
        {
            T[] array = new T[Count - index];
            Buffer.BlockCopy(_items, index, array, 0, (Count - index) * _sizeOf);
            return array;
        }

        /// <summary>
        ///     Convert this object into an array representation.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <param name="count"> Number of. </param>
        /// <returns>
        ///     An array that represents the data in this object.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray(int index, int count)
        {
            T[] array = new T[count];
            Buffer.BlockCopy(_items, index, array, 0, count * _sizeOf);
            return array;
        }

        /// <summary>
        ///     Ensures that capacity.
        /// </summary>
        /// <param name="min"> The minimum. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = _items.Length == 0 ? DEFAULT_CAPACITY : _items.Length * 2;
                if (newCapacity > MAX_CAPACITY) { newCapacity = MAX_CAPACITY; }
                if (newCapacity < min) { newCapacity          = min; }

                T[] newItems = new T[newCapacity];
                if (Count > 0) { Buffer.BlockCopy(_items, 0, newItems, 0, Count * _sizeOf); }
                _items = newItems;
            }
        }
    }
}