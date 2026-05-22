using NUnit.Framework;
using System;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class SerializableGuidTests
    {
        [Test]
        public void NewGuid_CreatesUniqueGuid()
        {
            var guid1 = SerializableGuid.NewGuid();
            var guid2 = SerializableGuid.NewGuid();
            Assert.AreNotEqual(guid1, guid2);
        }

        [Test]
        public void NewGuid_IsNotEmpty()
        {
            var guid = SerializableGuid.NewGuid();
            Assert.AreNotEqual(SerializableGuid.Empty, guid);
        }

        [Test]
        public void Empty_HasAllZeroParts()
        {
            Assert.AreEqual(0u, SerializableGuid.Empty.Part1);
            Assert.AreEqual(0u, SerializableGuid.Empty.Part2);
            Assert.AreEqual(0u, SerializableGuid.Empty.Part3);
            Assert.AreEqual(0u, SerializableGuid.Empty.Part4);
        }

        [Test]
        public void Constructor_WithParts_SetsCorrectly()
        {
            var guid = new SerializableGuid(1, 2, 3, 4);
            Assert.AreEqual(1u, guid.Part1);
            Assert.AreEqual(2u, guid.Part2);
            Assert.AreEqual(3u, guid.Part3);
            Assert.AreEqual(4u, guid.Part4);
        }

        [Test]
        public void Equals_SameParts_ReturnsTrue()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(1, 2, 3, 4);
            Assert.IsTrue(guid1 == guid2);
        }

        [Test]
        public void Equals_DifferentParts_ReturnsFalse()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(1, 2, 3, 5);
            Assert.IsFalse(guid1 == guid2);
        }

        [Test]
        public void NotEquals_DifferentParts_ReturnsTrue()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(5, 6, 7, 8);
            Assert.IsTrue(guid1 != guid2);
        }

        [Test]
        public void ToGuid_RoundTrip_PreservesValue()
        {
            var original = SerializableGuid.NewGuid();
            var systemGuid = original.ToGuid();
            var restored = new SerializableGuid(systemGuid);
            Assert.AreEqual(original, restored);
        }

        [Test]
        public void ToShortString_ProducesUrlSafeBase64()
        {
            var guid = SerializableGuid.NewGuid();
            string shortStr = guid.ToShortString();

            Assert.IsFalse(shortStr.Contains("+"), "Should not contain '+'");
            Assert.IsFalse(shortStr.Contains("/"), "Should not contain '/'");
            Assert.IsFalse(shortStr.EndsWith("="), "Should not end with '='");
        }

        [Test]
        public void FromShortString_RoundTrip_PreservesValue()
        {
            var original = SerializableGuid.NewGuid();
            string shortStr = original.ToShortString();
            var restored = SerializableGuid.FromShortString(shortStr);
            Assert.AreEqual(original, restored);
        }

        [Test]
        public void FromShortString_WithNull_ReturnsEmpty()
        {
            Assert.AreEqual(SerializableGuid.Empty, SerializableGuid.FromShortString(null));
        }

        [Test]
        public void FromShortString_WithEmptyString_ReturnsEmpty()
        {
            Assert.AreEqual(SerializableGuid.Empty, SerializableGuid.FromShortString(""));
        }

        [Test]
        public void ToString_ReturnsSystemGuidFormat()
        {
            var guid = SerializableGuid.NewGuid();
            string str = guid.ToString();
            Assert.IsTrue(str.Contains("-"), "Should be in standard GUID format with hyphens");
        }

        [Test]
        public void GetHashCode_SameGuids_SameHashCode()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(1, 2, 3, 4);
            Assert.AreEqual(guid1.GetHashCode(), guid2.GetHashCode());
        }

        [Test]
        public void GetHashCode_DifferentGuids_DifferentHashCode()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(5, 6, 7, 8);
            Assert.AreNotEqual(guid1.GetHashCode(), guid2.GetHashCode());
        }

        [Test]
        public void ImplicitConversion_FromSystemGuid_Works()
        {
            System.Guid systemGuid = System.Guid.NewGuid();
            SerializableGuid serializable = systemGuid;
            Assert.AreEqual(systemGuid, serializable.ToGuid());
        }

        [Test]
        public void ImplicitConversion_ToSystemGuid_Works()
        {
            var serializable = SerializableGuid.NewGuid();
            System.Guid systemGuid = serializable;
            Assert.AreEqual(serializable.ToGuid(), systemGuid);
        }

        [Test]
        public void Equals_ObjectType_WithSameGuid_ReturnsTrue()
        {
            var guid1 = new SerializableGuid(1, 2, 3, 4);
            var guid2 = new SerializableGuid(1, 2, 3, 4);
            Assert.IsTrue(guid1.Equals((object)guid2));
        }

        [Test]
        public void Equals_ObjectType_WithDifferentType_ReturnsFalse()
        {
            var guid = new SerializableGuid(1, 2, 3, 4);
            Assert.IsFalse(guid.Equals("not a guid"));
        }
    }
}
