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

#pragma warning disable 1591

namespace Exomia.Framework.Collections
{
    public sealed class LinkedList<T> : IEnumerable<T>
    {
        private LinkedListNode _head;

        public uint Count { get; private set; }

        public LinkedListNode First
        {
            get { return _head; }
        }

        public LinkedListNode Last
        {
            get { return _head?.Previous; }
        }

        public LinkedList() { }

        public LinkedList(IEnumerable<T> collection)
        {
            if (collection == null) { throw new ArgumentNullException(nameof(collection)); }
            foreach (T item in collection)
            {
                AddLast(item);
            }
        }

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

        public void Clear()
        {
            _head = null;
            Count = 0;
        }

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

        public void RemoveFirst()
        {
            if (_head == null) { throw new InvalidOperationException("the linked list is empty."); }
            Remove(_head);
        }

        public void RemoveLast()
        {
            if (_head == null) { throw new InvalidOperationException("the linked list is empty."); }
            Remove(_head.Previous);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public sealed class LinkedListNode
        {
            public T Item;
            public LinkedListNode Next { get; internal set; }
            public LinkedListNode Previous { get; internal set; }

            internal LinkedListNode(in T value)
            {
                Item = value;
            }

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

        public struct Enumerator : IEnumerator<T>
        {
            private readonly LinkedList<T> _list;
            private LinkedListNode _node;
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