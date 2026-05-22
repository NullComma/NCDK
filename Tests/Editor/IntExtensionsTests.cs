using NUnit.Framework;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class IntExtensionsTests
    {
        [Test]
        public void CAbs_Negative_ReturnsPositive()
        {
            Assert.AreEqual(5, (-5).CAbs());
        }

        [Test]
        public void CAbs_Positive_ReturnsSame()
        {
            Assert.AreEqual(3, 3.CAbs());
        }

        [Test]
        public void CAbs_Zero_ReturnsZero()
        {
            Assert.AreEqual(0, 0.CAbs());
        }

        [Test]
        public void CClamp_WithinRange_ReturnsValue()
        {
            Assert.AreEqual(5, 5.CClamp(0, 10));
        }

        [Test]
        public void CClamp_BelowMin_ReturnsMin()
        {
            Assert.AreEqual(0, (-5).CClamp(0, 10));
        }

        [Test]
        public void CClamp_AboveMax_ReturnsMax()
        {
            Assert.AreEqual(10, 15.CClamp(0, 10));
        }

        [Test]
        public void CClamp01_Negative_ReturnsZero()
        {
            Assert.AreEqual(0, (-1).CClamp01());
        }

        [Test]
        public void CClamp01_Positive_ReturnsOne()
        {
            Assert.AreEqual(1, 5.CClamp01());
        }

        [Test]
        public void CClamp01_Zero_ReturnsZero()
        {
            Assert.AreEqual(0, 0.CClamp01());
        }

        [Test]
        public void CClamp01_One_ReturnsOne()
        {
            Assert.AreEqual(1, 1.CClamp01());
        }
    }
}
