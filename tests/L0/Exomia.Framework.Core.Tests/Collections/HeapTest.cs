#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Core.Collections;
using Exomia.Framework.Core.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exomia.Framework.Tests.Collections
{
    [TestClass]
    public class HeapTest
    {
        [TestMethod]
        public void AddThanRemoveFirst_GivenKnownValues_ExpectedCorrectOrderLowestToHighest()
        {
            Heap<int> heap = new Heap<int>(100, (in int i1, in int i2) => i1.CompareTo(i2));

            heap.Add(20);
            heap.Add(9);
            heap.Add(5);
            heap.Add(12);
            heap.Add(68);
            heap.Add(7);
            heap.Add(10);
            heap.Add(7);
            heap.Add(42);
            heap.Add(15);

            Assert.AreEqual(10, heap.Count);

            Assert.AreEqual(5,  heap.RemoveFirst());
            Assert.AreEqual(7,  heap.RemoveFirst());
            Assert.AreEqual(7,  heap.RemoveFirst());
            Assert.AreEqual(9,  heap.RemoveFirst());
            Assert.AreEqual(10, heap.RemoveFirst());

            Assert.AreEqual(5, heap.Count);

            Assert.AreEqual(12, heap.RemoveFirst());
            Assert.AreEqual(15, heap.RemoveFirst());
            Assert.AreEqual(20, heap.RemoveFirst());
            Assert.AreEqual(42, heap.RemoveFirst());
            Assert.AreEqual(68, heap.RemoveFirst());

            Assert.AreEqual(0, heap.Count);
        }

        [TestMethod]
        public void AddThanRemoveFirst_GivenKnownValuesAndZero_ExpectedCorrectOrderLowestToHighest()
        {
            Heap<int> heap = new Heap<int>(100, (in int i1, in int i2) => i1.CompareTo(i2));

            heap.Add(20);
            heap.Add(0);
            heap.Add(5);
            heap.Add(12);
            heap.Add(68);
            heap.Add(7);
            heap.Add(10);
            heap.Add(7);
            heap.Add(0);
            heap.Add(15);

            Assert.AreEqual(10, heap.Count);

            Assert.AreEqual(0, heap.RemoveFirst());
            Assert.AreEqual(0, heap.RemoveFirst());
            Assert.AreEqual(5, heap.RemoveFirst());
            Assert.AreEqual(7, heap.RemoveFirst());
            Assert.AreEqual(7, heap.RemoveFirst());

            Assert.AreEqual(5, heap.Count);

            Assert.AreEqual(10, heap.RemoveFirst());
            Assert.AreEqual(12, heap.RemoveFirst());
            Assert.AreEqual(15, heap.RemoveFirst());
            Assert.AreEqual(20, heap.RemoveFirst());
            Assert.AreEqual(68, heap.RemoveFirst());

            Assert.AreEqual(0, heap.Count);
        }

        [DataTestMethod]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(1000)]
        public void AddThanRemoveFirst_GivenRandomValuesBetweenOneAndCapacity_ExpectedCorrectOrderLowestToHighest(
            int capacity)
        {
            Heap<int> heap = new Heap<int>(capacity, (in int i1, in int i2) => i1.CompareTo(i2));

            int[] items = new int[capacity];
            for (int i = 0; i < items.Length; i++)
            {
                heap.Add(items[i] = Random2.Default.Next(1, capacity));
            }

            Assert.AreEqual(capacity, heap.Count);
            Array.Sort(items);

            for (int i = 0; i < items.Length; i++)
            {
                Assert.AreEqual(items[i], heap.RemoveFirst());
            }

            Assert.AreEqual(0, heap.Count);
        }

        [DataTestMethod]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(1000)]
        public void
            AddThanRemoveFirst_GivenRandomValuesBetweenNegativeCapacityAndCapacity_ExpectedCorrectOrderLowestToHighest(
                int capacity)
        {
            Heap<int> heap = new Heap<int>(capacity, (in int i1, in int i2) => i1.CompareTo(i2));

            int[] items = new int[capacity];
            for (int i = 0; i < items.Length; i++)
            {
                heap.Add(items[i] = Random2.Default.Next(-capacity, capacity));
            }

            Assert.AreEqual(capacity, heap.Count);
            Array.Sort(items);

            for (int i = 0; i < items.Length; i++)
            {
                Assert.AreEqual(items[i], heap.RemoveFirst());
            }

            Assert.AreEqual(0, heap.Count);
        }

        [DataTestMethod]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(1000)]
        public void
            AddThanRemoveFirst_GivenDoubleInitialCapacityRandomValuesBetweenNegativeCapacityAndCapacity_ExpectedCorrectOrderLowestToHighest(
                int capacity)
        {
            Heap<int> heap = new Heap<int>(capacity, (in int i1, in int i2) => i1.CompareTo(i2));

            int[] items = new int[capacity * 2];
            for (int i = 0; i < items.Length; i++)
            {
                heap.Add(items[i] = Random2.Default.Next(-capacity, capacity));
            }

            Assert.AreEqual(capacity * 2, heap.Count);
            Array.Sort(items);

            for (int i = 0; i < items.Length; i++)
            {
                Assert.AreEqual(items[i], heap.RemoveFirst());
            }

            Assert.AreEqual(0, heap.Count);
        }

        [DataTestMethod]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(1000)]
        public void AddThanRemoveFirst_GivenRandomValues_ExpectedCorrectOrderLowestToHighest(int capacity)
        {
            Heap<int> heap = new Heap<int>(capacity, (in int i1, in int i2) => i1.CompareTo(i2));

            int[] items = new int[capacity];
            for (int i = 0; i < items.Length; i++)
            {
                heap.Add(items[i] = Random2.Default.Next());
            }

            Assert.AreEqual(capacity, heap.Count);
            Array.Sort(items);

            for (int i = 0; i < items.Length; i++)
            {
                Assert.AreEqual(items[i], heap.RemoveFirst());
            }

            Assert.AreEqual(0, heap.Count);
        }
    }
}