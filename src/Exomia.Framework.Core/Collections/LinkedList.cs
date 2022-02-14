#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections;

namespace Exomia.Framework.Core.Collections
{
    /// <summary> List of linked. This class cannot be inherited. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public sealed class LinkedList<T> : IEnumerable<T>
    {
        /// <summary> Gets the number of. </summary>
        /// <value> The count. </value>
        public uint Count { get; private set; }

        /// <summary> Gets the first. </summary>
        /// <value> The first. </value>
        public LinkedListNode? First { get; private set; }

        /// <summary> Gets the last. </summary>
        /// <value> The last. </value>
        public LinkedListNode? Last
        {
            get { return First?.Previous; }
        }

        /// <summary> Initializes a new instance of the <see cref="LinkedList{T}" /> class. </summary>
        public LinkedList() { }

        /// <summary> Initializes a new instance of the <see cref="LinkedList{T}" /> class. </summary>
        /// <param name="collection"> The collection. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public LinkedList(IEnumerable<T> collection)
        {
            if (collection == null) { throw new ArgumentNullException(nameof(collection)); }
            foreach (T item in collection)
            {
                AddLast(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary> Adds a first. </summary>
        /// <param name="item"> The item. </param>
        /// <returns> A list of. </returns>
        public LinkedListNode AddFirst(in T item)
        {
            LinkedListNode node = new LinkedListNode(item);
            if (First == null)
            {
                node.Next     = node;
                node.Previous = node;
                First         = node;
            }
            else
            {
                node.Next     = First;
                node.Previous = First.Previous;

                First.Previous!.Next = node;
                First.Previous       = node;

                First = node;
            }
            Count++;
            return node;
        }

        /// <summary> Adds a last. </summary>
        /// <param name="item"> The item. </param>
        /// <returns> A list of. </returns>
        public LinkedListNode AddLast(in T item)
        {
            LinkedListNode node = new LinkedListNode(item);
            if (First == null)
            {
                node.Next     = node;
                node.Previous = node;
                First         = node;
            }
            else
            {
                node.Next     = First;
                node.Previous = First.Previous;

                First.Previous!.Next = node;
                First.Previous       = node;
            }
            Count++;
            return node;
        }

        /// <summary> Clears this object to its blank/initial state. </summary>
        public void Clear()
        {
            First = null;
            Count = 0;
        }

        /// <summary> Enumerates as enumerable in this collection. </summary>
        /// <returns> An enumerator that allows foreach to be used to process as enumerable in this collection. </returns>
        public IEnumerable<LinkedListNode> AsEnumerable()
        {
            if (First != null)
            {
                LinkedListNode node = First;
                do
                {
                    yield return node;
                    node = node.Next!;
                }
                while (node != First);
            }
        }

        /// <summary> Applies an operation to all items in this collection. </summary>
        /// <param name="action"> The action. </param>
        public void ForEach(Action<LinkedListNode> action)
        {
            if (action != null && First != null)
            {
                LinkedListNode node = First;
                do
                {
                    action(node);
                    node = node.Next!;
                }
                while (node != First);
            }
        }

        /// <summary> Removes the given node. </summary>
        /// <param name="node"> The node to remove. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public void Remove(LinkedListNode node)
        {
            if (node == null) { throw new ArgumentNullException(nameof(node)); }
            if (node.Next == node) { First = null; }
            else
            {
                node.Previous!.Next = node.Next;
                node.Next!.Previous = node.Previous;
                if (First == node) { First = node.Next; }
            }
            node.Invalidate();
            Count--;
        }

        /// <summary> Removes the first. </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        public void RemoveFirst()
        {
            if (First == null) { throw new InvalidOperationException("the linked list is empty."); }
            Remove(First);
        }

        /// <summary> Removes the last. </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        public void RemoveLast()
        {
            if (First == null) { throw new InvalidOperationException("the linked list is empty."); }
            Remove(First.Previous!);
        }

        /// <summary> Gets the enumerator. </summary>
        /// <returns> The enumerator. </returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary> A linked list node. This class cannot be inherited. </summary>
        public sealed class LinkedListNode
        {
            /// <summary> The item. </summary>
            public T Item;

            /// <summary> Gets the next. </summary>
            /// <value> The next. </value>
            public LinkedListNode? Next { get; internal set; }

            /// <summary> Gets the previous. </summary>
            /// <value> The previous. </value>
            public LinkedListNode? Previous { get; internal set; }

            internal LinkedListNode(in T value)
            {
                Item = value;
            }

            /// <summary> Implicit cast that converts the given LinkedListNode to a T. </summary>
            /// <param name="node"> The node. </param>
            /// <returns> The result of the operation. </returns>
            public static implicit operator T(LinkedListNode node)
            {
                return node.Item;
            }

            internal void Invalidate()
            {
                Next     = null;
                Previous = null;
            }
        }

        /// <summary> An enumerator. </summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly LinkedList<T>   _list;
            private          LinkedListNode? _node;

            /// <inheritdoc />
            public T Current { get; private set; }

            /// <inheritdoc />
            object IEnumerator.Current
            {
                get { return Current!; }
            }

            /// <summary> Initializes a new instance of the &lt;see cref="LinkedList&lt;T&gt;"/&gt; class. </summary>
            /// <param name="list"> The list. </param>
            public Enumerator(LinkedList<T> list)
            {
                _list   = list;
                _node   = list.First;
                Current = default!;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_node == null)
                {
                    return false;
                }
                Current = _node.Item;
                _node   = _node.Next;
                if (_node == _list.First)
                {
                    _node = null;
                }
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                Current = default!;
                _node   = _list.First;
            }

            /// <inheritdoc />
            public void Dispose() { }
        }
    }
}