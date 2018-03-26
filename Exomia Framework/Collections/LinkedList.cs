#pragma warning disable 1591

using System;
using System.Collections.Generic;

namespace Exomia.Framework.Collections
{
    public sealed class LinkedList<T>
    {
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
                Item = default;
                Next = null;
                Previous = null;
            }
        }

        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        #endregion

        #region Properties

        #region Statics

        #endregion

        public uint Count { get; private set; }

        public LinkedListNode First
        {
            get { return Last?.Next; }
        }

        public LinkedListNode Last { get; private set; }

        #endregion

        #region Constructors

        #region Statics

        #endregion

        public LinkedList() { }

        public LinkedList(IEnumerable<T> collection)
        {
            if (collection == null) { throw new ArgumentNullException(nameof(collection)); }
            foreach (T item in collection)
            {
                AddLast(item);
            }
        }

        #endregion

        #region Methods

        #region Statics

        #endregion

        public LinkedListNode AddFirst(in T item)
        {
            LinkedListNode node = new LinkedListNode(item);
            if (Last == null)
            {
                node.Next = node;
                node.Previous = node;
                Last = node;
            }
            else
            {
                node.Next = Last.Next;
                node.Previous = Last;
                Last.Next.Previous = node;
                Last.Next = node;
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
            Last = null;
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

        #endregion
    }
}