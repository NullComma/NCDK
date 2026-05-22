using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class ColorExtensionsTests
    {
        [Test]
        public void SetAlpha_SetsCorrectAlpha()
        {
            var color = new Color(1f, 0.5f, 0f, 1f);
            var result = color.SetAlpha(0.25f);
            Assert.AreEqual(0.25f, result.a, 0.001f);
            Assert.AreEqual(1f, result.r, 0.001f);
            Assert.AreEqual(0.5f, result.g, 0.001f);
            Assert.AreEqual(0f, result.b, 0.001f);
        }

        [Test]
        public void SetAlpha_ZeroAlpha_Transparent()
        {
            var color = new Color(1f, 1f, 1f, 1f);
            var result = color.SetAlpha(0f);
            Assert.AreEqual(0f, result.a, 0.001f);
        }

        [Test]
        public void SetAlpha_DoesNotModifyOriginal()
        {
            var original = new Color(1f, 0f, 0f, 1f);
            original.SetAlpha(0.5f);
            Assert.AreEqual(1f, original.a, 0.001f, "SetAlpha should not modify the original color");
        }

        [Test]
        public void CLerp_AtHalf_ReturnsMidColor()
        {
            var start = new Color(0f, 0f, 0f);
            var end = new Color(1f, 1f, 1f);
            var result = start.CLerp(end, 0.5f);
            Assert.AreEqual(0.5f, result.r, 0.001f);
            Assert.AreEqual(0.5f, result.g, 0.001f);
            Assert.AreEqual(0.5f, result.b, 0.001f);
        }

        [Test]
        public void CLerp_AtZero_ReturnsStart()
        {
            var start = new Color(0.2f, 0.4f, 0.6f);
            var end = new Color(0.8f, 0.9f, 1.0f);
            var result = start.CLerp(end, 0f);
            Assert.AreEqual(start, result);
        }

        [Test]
        public void CLerp_AtOne_ReturnsEnd()
        {
            var start = new Color(0.2f, 0.4f, 0.6f);
            var end = new Color(0.8f, 0.9f, 1.0f);
            var result = start.CLerp(end, 1f);
            Assert.AreEqual(end, result);
        }
    }
}
