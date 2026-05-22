using NUnit.Framework;
using System.Linq;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        enum TestEnum { ValueA = 0, ValueB = 1, ValueC = 2 }

        [Test]
        public void CGetValues_ReturnsAllValues()
        {
            var values = EnumExtensions.CGetValues<TestEnum>().ToList();
            Assert.AreEqual(3, values.Count);
            Assert.Contains(TestEnum.ValueA, values);
            Assert.Contains(TestEnum.ValueB, values);
            Assert.Contains(TestEnum.ValueC, values);
        }

        [Test]
        public void CToInt_ReturnsIntegerValue()
        {
            Assert.AreEqual(1, TestEnum.ValueB.CToInt());
        }

        [Test]
        public void CToUInt_ReturnsUnsignedIntegerValue()
        {
            Assert.AreEqual(0u, TestEnum.ValueA.CToUInt());
            Assert.AreEqual(2u, TestEnum.ValueC.CToUInt());
        }

        [Test]
        public void CGetMaxValue_ReturnsHighestValue()
        {
            Assert.AreEqual(TestEnum.ValueC, EnumExtensions.CGetMaxValue<TestEnum>());
        }
    }
}
