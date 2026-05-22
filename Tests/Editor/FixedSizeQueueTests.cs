using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class FixedSizeQueueTests
    {
        [Test]
        public void Constructor_WithValidLimit_SetsLimit()
        {
            var queue = new CFixedSizedQueue<int>(5);
            Assert.AreEqual(5, queue.Limit);
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void Constructor_WithZeroLimit_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CFixedSizedQueue<int>(0));
        }

        [Test]
        public void Constructor_WithNegativeLimit_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CFixedSizedQueue<int>(-1));
        }

        [Test]
        public void Constructor_WithCollection_SetsLimitToCollectionCount()
        {
            var collection = new List<int> { 1, 2, 3 };
            var queue = new CFixedSizedQueue<int>(collection);
            Assert.AreEqual(3, queue.Limit);
            Assert.AreEqual(3, queue.Count);
        }

        [Test]
        public void Constructor_WithNullCollection_ThrowsArgumentException()
        {
            List<int> nullCollection = null;
            Assert.Throws<ArgumentException>(() => new CFixedSizedQueue<int>(nullCollection));
        }

        [Test]
        public void Constructor_WithEmptyCollection_ThrowsArgumentException()
        {
            var emptyCollection = new List<int>();
            Assert.Throws<ArgumentException>(() => new CFixedSizedQueue<int>(emptyCollection));
        }

        [Test]
        public void Enqueue_AddsItemsUpToLimit()
        {
            var queue = new CFixedSizedQueue<int>(3);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.AreEqual(3, queue.Count);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, queue.ToArray());
        }

        [Test]
        public void Enqueue_ExceedingLimit_RemovesOldest()
        {
            var queue = new CFixedSizedQueue<int>(3);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);

            Assert.AreEqual(3, queue.Count);
            CollectionAssert.AreEqual(new[] { 2, 3, 4 }, queue.ToArray());
        }

        [Test]
        public void Enqueue_MultipleOverLimit_KeepsLastN()
        {
            var queue = new CFixedSizedQueue<int>(3);
            for (int i = 1; i <= 10; i++)
            {
                queue.Enqueue(i);
            }

            Assert.AreEqual(3, queue.Count);
            CollectionAssert.AreEqual(new[] { 8, 9, 10 }, queue.ToArray());
        }

        [Test]
        public void Clear_RemovesAllItems()
        {
            var queue = new CFixedSizedQueue<int>(5);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Clear();

            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void GetEnumerator_ReturnsItemsInOrder()
        {
            var queue = new CFixedSizedQueue<string>(3);
            queue.Enqueue("a");
            queue.Enqueue("b");
            queue.Enqueue("c");

            var items = queue.ToList();
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, items);
        }

        [Test]
        public void GetEnumerator_DoesNotMutateQueue()
        {
            var queue = new CFixedSizedQueue<int>(3);
            queue.Enqueue(1);
            queue.Enqueue(2);

            var _ = queue.ToList();
            Assert.AreEqual(2, queue.Count);
        }
    }
}
