#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
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
using System.Collections;
using System.Collections.Generic;

namespace Exomia.Framework.Collections
{
    /// <summary>
    ///     List of linked. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public sealed class LinkedList<T> : IEnumerable<T>
    {
        /// <summary>
        ///     The head.
        /// </summary>
        private LinkedListNode _head;

        /// <summary>
        ///     Gets the number of.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public uint Count { get; private set; }

        /// <summary>
        ///     Gets the first.
        /// </summary>
        /// <value>
        ///     The first.
        /// </value>
        public LinkedListNode First
        {
            get { return _head; }
        }

        /// <summary>
        ///     Gets the last.
        /// </summary>
        /// <value>
        ///     The last.
        /// </value>
        public LinkedListNode Last
        {
            get { return _head?.Previous; }
        }

        /// <summary>
        ///     Initializes a new instance of the &lt;see cref="LinkedList&lt;T&gt;"/&gt; class.
        /// </summary>
        public LinkedList() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinkedList{T}" /> class.
        /// </summary>
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

        /// <summary>
        ///     Gets the enumerator.
        /// </summary>
        /// <returns>
        ///     The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Adds a first.
        /// </summary>
        /// <param name="item"> The item. </param>
        /// <returns>
        ///     A list of.
        /// </returns>
        public LinkedListNode AddFirst(in T item)
        {
            LinkedListNode node = new LinkedListNode(item);
            if (_head == null)
            {
                node.Next     = node;
                node.Previous = node;
                _head         = node;
            }
            else
            {
                node.Next     = _head;
                node.Previous = _head.Previous;

                _head.Previous.Next = node;
                _head.Previous      = node;

                _head = node;
            }
            Count++;
            return node;
        }

        /// <summary>
        ///     Adds a last.
        /// </summary>
        /// <param name="item"> The item. </param>
        /// <returns>
        ///     A list of.
        /// </returns>
        public LinkedListNode AddLast(in T item)
        {
            LinkedListNode node = new LinkedListNode(item);
            if (_head == null)
            {
                node.Next     = node;
                node.Previous = node;
                _head         = node;
            }
            else
            {
                node.Next     = _head;
                node.Previous = _head.Previous;

                _head.Previous.Next = node;
                _head.Previous      = node;
            }
            Count++;
            return node;
        }

        /// <summary>
        ///     Clears this object to its blank/initial state.
        /// </summary>
        public void Clear()
        {
            _head = null;
            Count = 0;
        }

        /// <summary>
        ///     Enumerates as enumerable in this collection.
        /// </summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process as enumerable in this collection.
        /// </returns>
        public IEnumerable<LinkedListNode> AsEnumerable()
        {
            if (_head != null)
            {
                LinkedListNode node = _head;
                do
                {
                    yield return node;
                    node = node.Next;
                } while (node != _head);
            }
        }

        /// <summary>
        ///     Applies an operation to all items in this collection.
        /// </summary>
        /// <param name="action"> The action. </param>
        public void ForEach(Action<LinkedListNode> action)
        {
            if (action != null && _head != null)
            {
                LinkedListNode node = _head;
                do
                {
                    action(node);
                    node = node.Next;
                } while (node != _head);
            }
        }

        /// <summary>
        ///     Removes the given node.
        /// </summary>
        /// <param name="node"> The node to remove. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public void Remove(LinkedListNode node)
        {
            if (node == null) { throw new ArgumentNullException(nameof(node)); }
            if (node.Next == node) { _head = null; }
            else
            {
                node.Previous.Next = node.Next;
                node.Next.Previous = node.Previous;
                if (_head == node) { _head = node.Next; }
            }
            node.Invalidate();
            Count--;
        }

        /// <summary>
        ///     Removes the first.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        public void RemoveFirst()
        {
            if (_head == null) { throw new InvalidOperationException("the linked list is empty."); }
            Remove(_head);
        }

        /// <summary>
        ///     Removes the last.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        public void RemoveLast()
        {
            if (_head == null) { throw new InvalidOperationException("the linked list is empty."); }
            Remove(_head.Previous);
        }

        /// <summary>
        ///     Gets the enumerator.
        /// </summary>
        /// <returns>
        ///     The enumerator.
        /// </returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        ///     A linked list node. This class cannot be inherited.
        /// </summary>
        public sealed class LinkedListNode
        {
            /// <summary>
            ///     The item.
            /// </summary>
            public T Item;

            /// <summary>
            ///     Gets the next.
            /// </summary>
            /// <value>
            ///     The next.
            /// </value>
            public LinkedListNode Next { get; internal set; }

            /// <summary>
            ///     Gets the previous.
            /// </summary>
            /// <value>
            ///     The previous.
            /// </value>
            public LinkedListNode Previous { get; internal set; }

            /// <summary>
            ///     Initializes a new instance of the <see cref="LinkedListNode" /> class.
            /// </summary>
            /// <param name="value"> The value. </param>
            internal LinkedListNode(in T value)
            {
                Item = value;
            }

            /// <summary>
            ///     Implicit cast that converts the given LinkedListNode to a T.
            /// </summary>
            /// <param name="node"> The node. </param>
            /// <returns>
            ///     The result of the operation.
            /// </returns>
            public static implicit operator T(LinkedListNode node)
            {
                return node.Item;
            }

            /// <summary>
            ///     Invalidates this object.
            /// </summary>
            internal void Invalidate()
            {
                Next     = null;
                Previous = null;
            }
        }

        /// <summary>
        ///     An enumerator.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            /// <summary>
            ///     The list.
            /// </summary>
            private readonly LinkedList<T> _list;

            /// <summary>
            ///     The node.
            /// </summary>
            private LinkedListNode _node;

            /// <summary>
            ///     The current.
            /// </summary>
            private T _current;

            /// <inheritdoc />
            public T Current
            {
                get { return _current; }
            }

            /// <inheritdoc />
            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            ///     Initializes a new instance of the &lt;see cref="LinkedList&lt;T&gt;"/&gt; class.
            /// </summary>
            /// <param name="list"> The list. </param>
            public Enumerator(LinkedList<T> list)
            {
                _list    = list;
                _node    = list._head;
                _current = default;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_node == null)
                {
                    return false;
                }
                _current = _node.Item;
                _node    = _node.Next;
                if (_node == _list._head)
                {
                    _node = null;
                }
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _current = default;
                _node    = _list._head;
            }

            /// <inheritdoc />
            public void Dispose() { }
        }
    }
}