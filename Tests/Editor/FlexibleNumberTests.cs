using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class FlexibleNumberTests
    {
        [Test]
        public void FlexibleFloat_ConstantMode_ReturnsSameValue()
        {
            FlexibleFloat flex = new FlexibleFloat(5.5f);
            Assert.AreEqual(5.5f, flex.GetValue());
            Assert.AreEqual(5.5f, flex.GetValue());
        }

        [Test]
        public void FlexibleFloat_RangeMode_ReturnsRandomValuesInRange()
        {
            float min = 5f;
            float max = 10f;
            FlexibleFloat flex = new FlexibleFloat(min, max);

            bool diffFound = false;
            float firstVal = flex.GetValue();
            
            for (int i = 0; i < 100; i++)
            {
                float val = flex.GetValue();
                Assert.IsTrue(val >= min && val <= max, $"Value {val} out of range [{min}, {max}]");
                if (Mathf.Abs(val - firstVal) > 0.0001f) diffFound = true;
            }

            Assert.IsTrue(diffFound, "Successive calls returned the same value in Range mode (unlikely but possible, try running again if it fails once).");
        }

        [Test]
        public void FlexibleFloat_StoredValue_StaysConsistent()
        {
            FlexibleFloat flex = new FlexibleFloat(5f, 10f);
            float stored = flex.GetValue();
            float call1 = stored;
            float call2 = stored;
            
            Assert.AreEqual(call1, call2, "Stored value should remain consistent.");
        }

        [Test]
        public void FlexibleInt_ConstantMode_ReturnsSameValue()
        {
            FlexibleInt flex = new FlexibleInt(7);
            Assert.AreEqual(7, flex.GetValue());
            Assert.AreEqual(7, flex.GetValue());
        }

        [Test]
        public void FlexibleInt_RangeMode_ReturnsRandomValuesInRange()
        {
            int min = 1;
            int max = 3; // 1, 2, or 3
            FlexibleInt flex = new FlexibleInt(min, max);

            bool found1 = false;
            bool found2 = false;
            bool found3 = false;

            for (int i = 0; i < 100; i++)
            {
                int val = flex.GetValue();
                Assert.IsTrue(val >= min && val <= max);
                if (val == 1) found1 = true;
                if (val == 2) found2 = true;
                if (val == 3) found3 = true;
            }

            Assert.IsTrue(found1 && found2 && found3, "Did not find all values (1, 2, 3) in range 1-3 inclusive.");
        }
    }
}
