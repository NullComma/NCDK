using NUnit.Framework;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class EnigmaPathsTests
    {
        [Test]
        public void SaveExtension_StartsWithDotEc()
        {
            string ext = EnigmaPaths.SaveExtension;
            Assert.IsTrue(ext.StartsWith(".ec"), "Extension should start with '.ec'");
        }

        [Test]
        public void SaveExtension_HasCorrectLength()
        {
            string ext = EnigmaPaths.SaveExtension;
            Assert.AreEqual(9, ext.Length); // .ec + 6 hex chars
        }

        [Test]
        public void SaveExtension_IsConsistent()
        {
            string ext1 = EnigmaPaths.SaveExtension;
            string ext2 = EnigmaPaths.SaveExtension;
            Assert.AreEqual(ext1, ext2, "Extension should be cached and consistent");
        }

        [Test]
        public void SaveExtension_IsLowerCase()
        {
            string ext = EnigmaPaths.SaveExtension;
            Assert.AreEqual(ext, ext.ToLowerInvariant(), "Extension should be lowercase");
        }

        [Test]
        public void SaveExtension_ContainsOnlyHexChars()
        {
            string ext = EnigmaPaths.SaveExtension.Substring(3); // remove ".ec"
            foreach (char c in ext)
            {
                Assert.IsTrue((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'),
                    $"Character '{c}' is not a valid hex digit");
            }
        }
    }
}
