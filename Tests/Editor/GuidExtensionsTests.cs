using NUnit.Framework;
using System;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class GuidExtensionsTests
    {
        [Test]
        public void ToSerializableGuid_RoundTrip_ReturnsOriginal()
        {
            var original = Guid.NewGuid();
            var serializable = original.ToSerializableGuid();
            var restored = serializable.ToSystemGuid();
            Assert.AreEqual(original, restored);
        }

        [Test]
        public void ToSerializableGuid_NotEmpty_ForNewGuid()
        {
            var original = Guid.NewGuid();
            var serializable = original.ToSerializableGuid();
            Assert.AreNotEqual(SerializableGuid.Empty, serializable);
        }

        [Test]
        public void ToSystemGuid_FromSerializableGuid_RoundTrip()
        {
            var original = SerializableGuid.NewGuid();
            var systemGuid = original.ToSystemGuid();
            var restored = new SerializableGuid(systemGuid);
            Assert.AreEqual(original, restored);
        }
    }
}
