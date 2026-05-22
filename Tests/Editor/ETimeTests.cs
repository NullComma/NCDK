using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class ETimeTests
    {
        [Test]
        public void DeltaTimeScaled_IsDeltaTimeTimesTimeScale()
        {
            float expected = Time.deltaTime * Time.timeScale;
            float actual = ETime.DeltaTimeScaled;
            Assert.AreEqual(expected, actual, 0.0001f);
        }

        [Test]
        public void TimeScale_Getter_ReturnsCurrentTimeScale()
        {
            float current = Time.timeScale;
            Assert.AreEqual(current, ETime.TimeScale, 0.0001f);
        }

        [Test]
        public void IsPaused_WhenTimeScaleIsZero_ReturnsTrue()
        {
            float original = Time.timeScale;
            Time.timeScale = 0f;
            Assert.IsTrue(ETime.IsPaused);
            Time.timeScale = original;
        }

        [Test]
        public void IsPaused_WhenTimeScaleIsOne_ReturnsFalse()
        {
            float original = Time.timeScale;
            Time.timeScale = 1f;
            Assert.IsFalse(ETime.IsPaused);
            Time.timeScale = original;
        }
    }
}
