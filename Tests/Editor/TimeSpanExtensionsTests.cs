using NUnit.Framework;
using System;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class TimeSpanExtensionsTests
    {
        [Test]
        public void FullTimeSpan_ReturnsAllComponents()
        {
            Assert.AreEqual("2d 5h 30m 15s", new TimeSpan(2, 5, 30, 15).CGetTimeSpanFormattedVerbose());
        }

        [Test]
        public void Zero_ReturnsEmpty()
        {
            Assert.AreEqual("", TimeSpan.Zero.CGetTimeSpanFormattedVerbose());
        }

        [Test]
        public void OnlySeconds_ReturnsSeconds()
        {
            // For sub-minute timespans, only seconds component is non-zero
            Assert.AreEqual("45s", new TimeSpan(0, 0, 0, 45).CGetTimeSpanFormattedVerbose());
        }

        [Test]
        public void LargeTimeSpan_ShowsAllComponents()
        {
            var result = new TimeSpan(7, 3, 15, 30).CGetTimeSpanFormattedVerbose();
            Assert.IsTrue(result.Contains("7d"), "Should contain days");
            Assert.IsTrue(result.Contains("3h"), "Should contain hours");
            Assert.IsTrue(result.Contains("15m"), "Should contain minutes");
            Assert.IsTrue(result.Contains("30s"), "Should contain seconds");
        }
    }
}
