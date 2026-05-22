using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class QuaternionExtensionsTests
    {
        [Test]
        public void CLerp_AtHalf_ReturnsMidRotation()
        {
            var start = Quaternion.Euler(0, 0, 0);
            var end = Quaternion.Euler(0, 90, 0);
            var result = start.CLerp(end, 0.5f);
            var euler = result.eulerAngles;
            Assert.AreEqual(45, euler.y, 1f);
        }

        [Test]
        public void CLerp_AtZero_ReturnsStart()
        {
            var start = Quaternion.Euler(10, 20, 30);
            var end = Quaternion.Euler(40, 50, 60);
            var result = start.CLerp(end, 0f);
            Assert.AreEqual(start.eulerAngles, result.eulerAngles);
        }

        [Test]
        public void CLerp_AtOne_ReturnsEnd()
        {
            var start = Quaternion.Euler(10, 20, 30);
            var end = Quaternion.Euler(40, 50, 60);
            var result = start.CLerp(end, 1f);
            Assert.AreEqual(end.eulerAngles, result.eulerAngles);
        }
    }
}
