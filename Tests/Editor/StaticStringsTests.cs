using NUnit.Framework;
using UnityEngine;

namespace EnigmaCore.Tests.Editor
{
    [TestFixture]
    public class StaticStringsTests
    {
        [Test]
        public void Builder_IsNotNull()
        {
            Assert.IsNotNull(StaticStrings.Builder);
        }

        [Test]
        public void PrefixScripts_IsCorrect()
        {
            Assert.AreEqual("EnigmaCore/", StaticStrings.PrefixScripts);
        }

        [Test]
        public void PrefixTools_IsCorrect()
        {
            Assert.AreEqual("Tools/EnigmaCore/", StaticStrings.PrefixTools);
        }

        [Test]
        public void DontDestroyOnLoad_IsCorrect()
        {
            Assert.AreEqual("DontDestroyOnLoad", StaticStrings.DontDestroyOnLoad);
        }

        [Test]
        public void ResourcesPath_IsCorrect()
        {
            Assert.AreEqual("Assets/Resources/", StaticStrings.ResourcesPath);
        }
    }
}
