using NUnit.Framework;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void IsNotNullOrEmpty_WithNonNullNonEmpty_ReturnsTrue()
        {
            Assert.IsTrue("hello".IsNotNullOrEmpty());
        }

        [Test]
        public void IsNotNullOrEmpty_WithEmptyString_ReturnsFalse()
        {
            Assert.IsFalse("".IsNotNullOrEmpty());
        }

        [Test]
        public void IsNotNullOrEmpty_WithNull_ReturnsFalse()
        {
            string nullStr = null;
            Assert.IsFalse(nullStr.IsNotNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_WithNull_ReturnsTrue()
        {
            string nullStr = null;
            Assert.IsTrue(nullStr.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_WithEmptyString_ReturnsTrue()
        {
            Assert.IsTrue("".IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_WithNonEmpty_ReturnsFalse()
        {
            Assert.IsFalse("hello".IsNullOrEmpty());
        }

        [Test]
        public void IsNotNullOrWhitespace_WithNonWhitespace_ReturnsTrue()
        {
            Assert.IsTrue("hello".IsNotNullOrWhitespace());
        }

        [Test]
        public void IsNotNullOrWhitespace_WithWhitespaceOnly_ReturnsFalse()
        {
            Assert.IsFalse("   ".IsNotNullOrWhitespace());
        }

        [Test]
        public void IsNullOrWhitespace_WithNull_ReturnsTrue()
        {
            string nullStr = null;
            Assert.IsTrue(nullStr.IsNullOrWhitespace());
        }

        [Test]
        public void IsNullOrWhitespace_WithEmptyString_ReturnsTrue()
        {
            Assert.IsTrue("".IsNullOrWhitespace());
        }

        [Test]
        public void IsNullOrWhitespace_WithWhitespaceOnly_ReturnsTrue()
        {
            Assert.IsTrue("   ".IsNullOrWhitespace());
        }

        [Test]
        public void Substring_WithValidRange_ReturnsSubstring()
        {
            Assert.AreEqual("ell", StringExtensions.Substring("hello", 1, 3));
        }

        [Test]
        public void Substring_LengthExceedsString_ReturnsFullString()
        {
            Assert.AreEqual("hello", StringExtensions.Substring("hello", 0, 100));
        }

        [Test]
        public void Substring_WithNullInput_ReturnsNull()
        {
            string nullStr = null;
            Assert.IsNull(StringExtensions.Substring(nullStr, 0, 5));
        }

        [Test]
        public void Substring_WithEmptyInput_ReturnsNull()
        {
            Assert.IsNull(StringExtensions.Substring("", 0, 5));
        }

        [Test]
        public void FirstLetterToUpperCase_WithLowercase_CapitalizesFirst()
        {
            Assert.AreEqual("Hello", "hello".FirstLetterToUpperCase());
        }

        [Test]
        public void FirstLetterToUpperCase_WithEmptyString_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, "".FirstLetterToUpperCase());
        }

        [Test]
        public void FirstLetterToUpperCase_WithNull_ReturnsEmpty()
        {
            string nullStr = null;
            Assert.AreEqual(string.Empty, nullStr.FirstLetterToUpperCase());
        }

        [Test]
        public void FirstLetterToUpperCase_WithAlreadyCapitalized_KeepsSame()
        {
            Assert.AreEqual("Hello", "Hello".FirstLetterToUpperCase());
        }

        [Test]
        public void RemoveDiacritics_RemovesAccents()
        {
            Assert.AreEqual("cafe", "café".RemoveDiacritics());
        }

        [Test]
        public void RemoveDiacritics_RemovesMultipleAccents()
        {
            Assert.AreEqual("naive resume", "naïve résumé".RemoveDiacritics());
        }
    }
}
