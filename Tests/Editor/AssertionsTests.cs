using NUnit.Framework;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class AssertionsTests
    {
        [Test]
        public void ThrowIfFalse_TrueCondition_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => true.ThrowIfFalse());
        }

        [Test]
        public void ThrowIfFalse_TrueCondition_Returns()
        {
            bool reached = false;
            true.ThrowIfFalse("test error");
            reached = true;
            Assert.IsTrue(reached, "Code after ThrowIfFalse(true) should execute");
        }

        [Test]
        public void CAssertIfFalse_TrueCondition_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => true.CAssertIfFalse());
        }
    }
}
