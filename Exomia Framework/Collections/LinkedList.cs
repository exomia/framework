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

// TODO: REDESIGN
namespace Exomia.Framework.Collections
{
    public sealed class LinkedList<T>
    {
        public uint Count { get; private set; }

        public LinkedListNode First
        {
            get { return Last?.Next; }
        }

        public LinkedListNode Last { get; private set; }

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
            if (Last == null)
            {
                node.Next     = node;
                node.Previous = node;
                Last          = node;
            }
            else
            {
                node.Next          = Last.Next;
                node.Previous      = Last;
                Last.Next.Previous = node;
                Last.Next          = node;
            }
            Count++;
            return node;
        }

        public LinkedListNode AddLast(in T item)
        {
            return Last = AddFirst(item);
        }

        public void RemoveFirst()
        {
            if (Last == null) { throw new InvalidOperationException("the linked list is empty."); }
            Remove(Last.Next);
        }

        public void RemoveLast()
        {
            if (Last == null) { throw new InvalidOperationException("the linked list is empty."); }
            Remove(Last);
        }

        public void Remove(LinkedListNode item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            if (item.Next == item) { Last = null; }
            else
            {
                item.Previous.Next = item.Next;
                item.Next.Previous = item.Previous;
                if (Last == item) { Last = item.Previous; }
            }
            item.Invalidate();
            Count--;
        }

        public void Clear()
        {
            Last  = null;
            Count = 0;
        }

        public IEnumerable<LinkedListNode> Enumerate()
        {
            if (Last != null)
            {
                LinkedListNode node = Last.Next;
                do
                {
                    yield return node;
                    node = node.Next;
                } while (node != Last.Next);
            }
        }

        public sealed class LinkedListNode
        {
            internal T Item;
            internal LinkedListNode Next;
            internal LinkedListNode Previous;

            internal LinkedListNode(in T value)
            {
                Item = value;
            }

            internal void Invalidate()
            {
                Item     = default;
                Next     = null;
                Previous = null;
            }
        }
    }
}