using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class IEnumerableExtensionsTests
    {
        [Test]
        public void RandomElement_WithNonEmptyList_ReturnsElement()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            int result = list.RandomElement();
            Assert.IsTrue(result >= 1 && result <= 5);
        }

        [Test]
        public void RandomElement_WithEmptyList_ReturnsDefault()
        {
            var list = new List<int>();
            Assert.AreEqual(0, list.RandomElement());
        }

        [Test]
        public void RandomElement_WithStringList_ReturnsDefaultForEmpty()
        {
            var list = new List<string>();
            Assert.IsNull(list.RandomElement());
        }

        [Test]
        public void ContainsIndex_ValidIndex_ReturnsTrue()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.IsTrue(list.ContainsIndex(0));
            Assert.IsTrue(list.ContainsIndex(2));
        }

        [Test]
        public void ContainsIndex_NegativeIndex_ReturnsFalse()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.IsFalse(list.ContainsIndex(-1));
        }

        [Test]
        public void ContainsIndex_OutOfRange_ReturnsFalse()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.IsFalse(list.ContainsIndex(3));
        }

        [Test]
        public void ContainsIndex_NullEnumerable_ReturnsFalse()
        {
            List<int> nullList = null;
            Assert.IsFalse(nullList.ContainsIndex(0));
        }

        [Test]
        public void GetAtIndexSafe_ValidIndex_ReturnsElement()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.AreEqual(10, list.GetAtIndexSafe(0));
            Assert.AreEqual(30, list.GetAtIndexSafe(2));
        }

        [Test]
        public void GetAtIndexSafe_InvalidIndex_ReturnsDefault()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.AreEqual(0, list.GetAtIndexSafe(5));
            Assert.AreEqual(0, list.GetAtIndexSafe(-1));
        }

        [Test]
        public void HasAnyAndNotNull_WithElements_ReturnsTrue()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.IsTrue(list.HasAnyAndNotNull());
        }

        [Test]
        public void HasAnyAndNotNull_WithNull_ReturnsFalse()
        {
            List<int> nullList = null;
            Assert.IsFalse(nullList.HasAnyAndNotNull());
        }

        [Test]
        public void HasAnyAndNotNull_WithEmpty_ReturnsFalse()
        {
            var list = new List<int>();
            Assert.IsFalse(list.HasAnyAndNotNull());
        }

        [Test]
        public void IsNullOrEmpty_WithNull_ReturnsTrue()
        {
            List<int> nullList = null;
            Assert.IsTrue(nullList.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_WithEmpty_ReturnsTrue()
        {
            var list = new List<int>();
            Assert.IsTrue(list.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_WithElements_ReturnsFalse()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.IsFalse(list.IsNullOrEmpty());
        }

        [Test]
        public void RemoveNulls_RemovesNullReferences()
        {
            var list = new List<string> { "a", null, "b", null, "c" };
            var result = list.RemoveNulls().ToList();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("c", result[2]);
        }

        [Test]
        public void RemoveNulls_WithNullEnumerable_ReturnsEmpty()
        {
            List<string> nullList = null;
            var result = nullList.RemoveNulls().ToList();
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void RemoveNulls_WithAllNulls_ReturnsEmpty()
        {
            var list = new List<string> { null, null };
            var result = list.RemoveNulls().ToList();
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void RemoveNulls_WithNoNulls_ReturnsAll()
        {
            var list = new List<int> { 1, 2, 3 };
            var result = list.RemoveNulls().ToList();
            Assert.AreEqual(3, result.Count);
        }
    }
}
