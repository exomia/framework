#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Collections
{
    /// <summary>
    ///     A heap. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public sealed class Heap<T>
    {
        /// <summary>
        ///     Compares two in T objects to determine their relative ordering.
        /// </summary>
        /// <param name="i1"> In t to be compared. </param>
        /// <param name="i2"> In t to be compared. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        public delegate int Compare(in T i1, in T i2);

        private readonly Compare _compare;
        private          T[]     _items;
        private          int     _size;

        /// <summary>
        ///     Gets the number of items in the heap.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count
        {
            get { return _size; }
        }

        /// <summary>
        ///     Initializes a new instance of the &lt;see cref="Heap&lt;T&gt;"/&gt; class.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        /// <param name="compare">  The compare function. </param>
        public Heap(int capacity, Compare compare)
        {
            _compare = compare;
            _items   = new T[capacity];
            _size    = 0;
        }

        /// <summary>
        ///     Adds an item to the heap.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        public void Add(T item)
        {
            if (_size >= _items.Length)
            {
                Array.Resize(ref _items, _items.Length * 2);
            }
            _items[_size] = item;
            SortUp(_size++);
        }

        /// <summary>
        ///     Removes the first item from the heap.
        /// </summary>
        /// <returns>
        ///     A T.
        /// </returns>
        public T RemoveFirst()
        {
            T item = _items[0];
            _items[0]     = _items[--_size];
            _items[_size] = default!;
            SortDown(0);
            return item;
        }

        private void SortUp(int index)
        {
            int pIndex;
            while (_compare(_items[index], _items[pIndex = (index - 1) / 2]) < 0)
            {
                T item = _items[index];
                _items[index]          = _items[pIndex];
                _items[index = pIndex] = item;
            }
        }

        private void SortDown(int index)
        {
            // ReSharper disable once TooWideLocalVariableScope
            int lIndex, rIndex;

            while ((lIndex = (index * 2) + 1) < _size)
            {
                if ((rIndex = (index * 2) + 2) < _size &&
                    _compare(_items[lIndex], _items[rIndex]) > 0)
                {
                    lIndex = rIndex;
                }

                if (_compare(_items[index], _items[lIndex]) > 0)
                {
                    T item = _items[index];
                    _items[index]          = _items[lIndex];
                    _items[index = lIndex] = item;
                }
                else
                {
                    return;
                }
            }
        }
    }
}