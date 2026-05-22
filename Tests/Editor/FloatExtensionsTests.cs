using NUnit.Framework;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class FloatExtensionsTests
    {
        [Test]
        public void Clamp_WithinRange_ReturnsSameValue()
        {
            Assert.AreEqual(5f, 5f.Clamp(0f, 10f));
        }

        [Test]
        public void Clamp_BelowMin_ReturnsMin()
        {
            Assert.AreEqual(0f, (-5f).Clamp(0f, 10f));
        }

        [Test]
        public void Clamp_AboveMax_ReturnsMax()
        {
            Assert.AreEqual(10f, 15f.Clamp(0f, 10f));
        }

        [Test]
        public void Clamp01_WithinRange_ReturnsSameValue()
        {
            Assert.AreEqual(0.5f, 0.5f.Clamp01());
        }

        [Test]
        public void Clamp01_Negative_ReturnsZero()
        {
            Assert.AreEqual(0f, (-1f).Clamp01());
        }

        [Test]
        public void Clamp01_GreaterThanOne_ReturnsOne()
        {
            Assert.AreEqual(1f, 2f.Clamp01());
        }

        [Test]
        public void ClampAngle_WithinRange_ReturnsSameValue()
        {
            Assert.AreEqual(45f, 45f.ClampAngle(-90f, 90f));
        }

        [Test]
        public void ClampAngle_Above360_WrapsThenClamps()
        {
            Assert.AreEqual(10f, 370f.ClampAngle(0f, 90f));
        }

        [Test]
        public void ClampAngle_BelowNegative360_WrapsThenClamps()
        {
            Assert.AreEqual(-10f, (-370f).ClampAngle(-90f, 90f));
        }

        [Test]
        public void Remap_FromZeroToOneToZeroToTen_ReturnsCorrectValue()
        {
            Assert.AreEqual(5f, 0.5f.Remap(0f, 1f, 0f, 10f));
        }

        [Test]
        public void Remap_FromZeroToHundredToZeroToOne_ReturnsCorrectValue()
        {
            Assert.AreEqual(0.75f, 75f.Remap(0f, 100f, 0f, 1f));
        }

        [Test]
        public void IsInRange_WithinRange_ReturnsTrue()
        {
            Assert.IsTrue(5f.IsInRange(0f, 10f));
        }

        [Test]
        public void IsInRange_AtBoundary_ReturnsTrue()
        {
            Assert.IsTrue(0f.IsInRange(0f, 10f));
            Assert.IsTrue(10f.IsInRange(0f, 10f));
        }

        [Test]
        public void IsInRange_OutsideRange_ReturnsFalse()
        {
            Assert.IsFalse((-1f).IsInRange(0f, 10f));
            Assert.IsFalse(11f.IsInRange(0f, 10f));
        }

        [Test]
        public void Abs_Negative_ReturnsPositive()
        {
            Assert.AreEqual(5f, (-5f).Abs());
        }

        [Test]
        public void Abs_Positive_ReturnsSame()
        {
            Assert.AreEqual(5f, 5f.Abs());
        }

        [Test]
        public void GetCloserValue_CloserToA_ReturnsA()
        {
            Assert.AreEqual(0f, 1f.GetCloserValue(0f, 10f));
        }

        [Test]
        public void GetCloserValue_CloserToB_ReturnsB()
        {
            Assert.AreEqual(10f, 9f.GetCloserValue(0f, 10f));
        }

        [Test]
        public void GetCloserValue_Equidistant_ReturnsB()
        {
            Assert.AreEqual(10f, 5f.GetCloserValue(0f, 10f));
        }

        [Test]
        public void Lerp_FromZeroToTen_ReturnsCorrectValue()
        {
            Assert.AreEqual(5f, 0f.Lerp(10f, 0.5f));
        }

        [Test]
        public void Lerp_AtZero_ReturnsStart()
        {
            Assert.AreEqual(0f, 0f.Lerp(10f, 0f));
        }

        [Test]
        public void Lerp_AtOne_ReturnsEnd()
        {
            Assert.AreEqual(10f, 0f.Lerp(10f, 1f));
        }

        [Test]
        public void Imprecise_TruncatesToDecimalPlaces()
        {
            Assert.AreEqual(3.141f, 3.14159f.Imprecise(3));
        }

        [Test]
        public void Imprecise_ZeroDecimalPlaces_Truncates()
        {
            Assert.AreEqual(3f, 3.9f.Imprecise(0));
        }

        [Test]
        public void Imprecise_NegativeDecimalPlaces_TreatsAsZero()
        {
            Assert.AreEqual(3f, 3.9f.Imprecise(-1));
        }

        [Test]
        public void Rounded_RoundsToDecimalPlaces()
        {
            Assert.AreEqual(3.142f, 3.14159f.Rounded(3));
        }

        [Test]
        public void Rounded_NegativeDecimalPlaces_TreatsAsZero()
        {
            Assert.AreEqual(4f, 3.9f.Rounded(-1));
        }
    }
}
