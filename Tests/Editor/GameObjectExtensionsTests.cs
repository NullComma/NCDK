using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class GameObjectExtensionsTests
    {
        [Test]
        public void CGetOrAddComponent_WithExistingComponent_ReturnsExisting()
        {
            var go = new GameObject("Test");
            var added = go.AddComponent<BoxCollider>();
            var result = go.CGetOrAddComponent<BoxCollider>();
            Assert.AreEqual(added, result);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CGetOrAddComponent_WithoutComponent_AddsNew()
        {
            var go = new GameObject("Test");
            var result = go.CGetOrAddComponent<BoxCollider>();
            Assert.IsNotNull(result);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CGetOrAddComponent_WithNullGO_ReturnsNull()
        {
            LogAssert.Expect(LogType.Error, "Cant get or add component from null GameObject!");
            GameObject go = null;
            Assert.IsNull(go.CGetOrAddComponent<BoxCollider>());
        }

        [Test]
        public void CUnparentAllChildren_RemovesParent()
        {
            var parent = new GameObject("Parent");
            var child = new GameObject("Child");
            child.transform.SetParent(parent.transform);

            parent.CUnparentAllChildren();

            Assert.AreEqual(0, parent.transform.childCount);
            Assert.IsNull(child.transform.parent);
            Object.DestroyImmediate(child);
            Object.DestroyImmediate(parent);
        }

        [Test]
        public void CUnparentAllChildren_WithNull_DoesNotThrow()
        {
            GameObject go = null;
            Assert.DoesNotThrow(() => go.CUnparentAllChildren());
        }
    }
}
