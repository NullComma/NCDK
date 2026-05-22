using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class Vector2ExtensionsTests
    {
        [Test]
        public void GetAbsBiggerValue_XGreater_ReturnsX()
        {
            Assert.AreEqual(5f, new Vector2(5f, 2f).GetAbsBiggerValue());
        }

        [Test]
        public void GetAbsBiggerValue_YGreater_ReturnsY()
        {
            Assert.AreEqual(8f, new Vector2(3f, -8f).GetAbsBiggerValue());
        }

        [Test]
        public void GetAbsBiggerValue_Equal_ReturnsX()
        {
            Assert.AreEqual(4f, new Vector2(4f, 4f).GetAbsBiggerValue());
        }

        [Test]
        public void ImpreciseEqualCompare_EqualVectors_ReturnsTrue()
        {
            Assert.IsTrue(new Vector2(1.23456f, 2.34567f).ImpreciseEqualCompare(new Vector2(1.23456f, 2.34567f)));
        }

        [Test]
        public void ImpreciseEqualCompare_DifferentVectors_ReturnsFalse()
        {
            Assert.IsFalse(new Vector2(1.0f, 2.0f).ImpreciseEqualCompare(new Vector2(1.5f, 2.0f)));
        }

        [Test]
        public void CastValuesToInt_TruncatesDecimals()
        {
            var result = new Vector2(1.7f, 2.3f).CastValuesToInt();
            Assert.AreEqual(new Vector2(1f, 2f), result);
        }

        [Test]
        public void IsZero_AllZero_ReturnsTrue()
        {
            Assert.IsTrue(Vector2.zero.IsZero());
        }

        [Test]
        public void IsZero_NonZero_ReturnsFalse()
        {
            Assert.IsFalse(new Vector2(1f, 0f).IsZero());
        }

        [Test]
        public void IsOne_AllOne_ReturnsTrue()
        {
            Assert.IsTrue(Vector2.one.IsOne());
        }

        [Test]
        public void IsOne_NonOne_ReturnsFalse()
        {
            Assert.IsFalse(new Vector2(1f, 2f).IsOne());
        }

        [Test]
        public void Lerp_FromZeroToOne_ReturnsMidpoint()
        {
            var result = Vector2.zero.Lerp(Vector2.one, 0.5f);
            Assert.AreEqual(new Vector2(0.5f, 0.5f), result);
        }

        [Test]
        public void Lerp_AtZero_ReturnsStart()
        {
            var result = new Vector2(2f, 4f).Lerp(new Vector2(10f, 20f), 0f);
            Assert.AreEqual(new Vector2(2f, 4f), result);
        }

        [Test]
        public void Lerp_AtOne_ReturnsEnd()
        {
            var result = new Vector2(2f, 4f).Lerp(new Vector2(10f, 20f), 1f);
            Assert.AreEqual(new Vector2(10f, 20f), result);
        }
    }
}
