using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class ObjectExtensionsTests
    {
        [Test]
        public void CGetNameSafe_WithObject_ReturnsName()
        {
            var go = new GameObject("MyName");
            Assert.AreEqual("MyName", go.CGetNameSafe());
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CGetNameSafe_WithNull_ReturnsFallback()
        {
            Object nullObj = null;
            Assert.AreEqual("null", nullObj.CGetNameSafe());
        }

        [Test]
        public void CGetNameSafe_WithNullAndCustomFallback_ReturnsCustom()
        {
            Object nullObj = null;
            Assert.AreEqual("custom_fallback", nullObj.CGetNameSafe("custom_fallback"));
        }
    }
}
