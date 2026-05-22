using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class Vector3ExtensionsTests
    {
        [Test]
        public void GetAbsBiggerValue_XGreater_ReturnsX()
        {
            Assert.AreEqual(10f, new Vector3(10f, 2f, 3f).GetAbsBiggerValue());
        }

        [Test]
        public void GetAbsBiggerValue_YGreater_ReturnsY()
        {
            Assert.AreEqual(7f, new Vector3(1f, -7f, 3f).GetAbsBiggerValue());
        }

        [Test]
        public void GetAbsBiggerValue_ZGreater_ReturnsZ()
        {
            Assert.AreEqual(9f, new Vector3(1f, 2f, 9f).GetAbsBiggerValue());
        }

        [Test]
        public void GetCloserFrom_WithMultiplePositions_ReturnsClosest()
        {
            var positions = new Vector3[] { new(0, 0, 100), new(0, 0, 5), new(0, 0, 50) };
            var from = Vector3.zero;
            var closest = positions.GetCloserFrom(from);
            Assert.AreEqual(new Vector3(0, 0, 5), closest);
        }

        [Test]
        public void GetCloserFrom_WithEmptyArray_ReturnsDefault()
        {
            var positions = new Vector3[0];
            Assert.AreEqual(default(Vector3), positions.GetCloserFrom(Vector3.zero));
        }

        [Test]
        public void GetCloserFrom_WithSinglePosition_ReturnsThatPosition()
        {
            var positions = new Vector3[] { new(5, 10, 15) };
            Assert.AreEqual(new Vector3(5, 10, 15), positions.GetCloserFrom(Vector3.zero));
        }

        [Test]
        public void ImpreciseEqualCompare_EqualVectors_ReturnsTrue()
        {
            var a = new Vector3(1.23456f, 2.34567f, 3.45678f);
            var b = new Vector3(1.23456f, 2.34567f, 3.45678f);
            Assert.IsTrue(a.ImpreciseEqualCompare(b));
        }

        [Test]
        public void ImpreciseEqualCompare_DifferentVectors_ReturnsFalse()
        {
            Assert.IsFalse(new Vector3(1, 2, 3).ImpreciseEqualCompare(new Vector3(1, 5, 3)));
        }

        [Test]
        public void CastValuesToInt_TruncatesDecimals()
        {
            var result = new Vector3(1.7f, 2.3f, 3.9f).CastValuesToInt();
            Assert.AreEqual(new Vector3(1f, 2f, 3f), result);
        }

        [Test]
        public void CAbs_NegativeValues_ReturnsPositive()
        {
            var result = new Vector3(-1f, -2f, -3f).CAbs();
            Assert.AreEqual(new Vector3(1f, 2f, 3f), result);
        }

        [Test]
        public void CAbs_PositiveValues_ReturnsSame()
        {
            var result = new Vector3(1f, 2f, 3f).CAbs();
            Assert.AreEqual(new Vector3(1f, 2f, 3f), result);
        }

        [Test]
        public void CIsZero_AllZero_ReturnsTrue()
        {
            Assert.IsTrue(Vector3.zero.CIsZero());
        }

        [Test]
        public void CIsZero_NonZero_ReturnsFalse()
        {
            Assert.IsFalse(new Vector3(0, 1, 0).CIsZero());
        }

        [Test]
        public void CIsOne_AllOne_ReturnsTrue()
        {
            Assert.IsTrue(Vector3.one.CIsOne());
        }

        [Test]
        public void CIsOne_NonOne_ReturnsFalse()
        {
            Assert.IsFalse(new Vector3(1, 1, 2).CIsOne());
        }

        [Test]
        public void CMagnitudeXZ_WithOnlyX_ReturnsCorrectMagnitude()
        {
            Assert.AreEqual(3f, new Vector3(3f, 0f, 0f).CMagnitudeXZ(), 0.001f);
        }

        [Test]
        public void CMagnitudeXZ_WithOnlyZ_ReturnsCorrectMagnitude()
        {
            Assert.AreEqual(4f, new Vector3(0f, 0f, 4f).CMagnitudeXZ(), 0.001f);
        }

        [Test]
        public void CMagnitudeXZ_ZeroXZ_ReturnsZero()
        {
            Assert.AreEqual(0f, new Vector3(0f, 100f, 0f).CMagnitudeXZ(), 0.001f);
        }

        [Test]
        public void CClosestPointIsFirstParameter_FirstCloser_ReturnsTrue()
        {
            Assert.IsTrue(new Vector3(0, 0, 0).CClosestPointIsFirstParameter(new Vector3(1, 0, 0), new Vector3(10, 0, 0)));
        }

        [Test]
        public void CClosestPointIsFirstParameter_SecondCloser_ReturnsFalse()
        {
            Assert.IsFalse(new Vector3(0, 0, 0).CClosestPointIsFirstParameter(new Vector3(10, 0, 0), new Vector3(1, 0, 0)));
        }

        [Test]
        public void CMultiplyBy_MultipliesComponentWise()
        {
            var result = new Vector3(2, 3, 4).CMultiplyBy(new Vector3(5, 6, 7));
            Assert.AreEqual(new Vector3(10, 18, 28), result);
        }
    }
}
