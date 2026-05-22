using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class UnityObjectExtensionsTests
    {
        [Test]
        public void CDestroy_WithNull_DoesNotThrow()
        {
            Object obj = null;
            Assert.DoesNotThrow(() => obj.CDestroy());
        }

        [Test]
        public void CDestroy_DestroysGameObject()
        {
            var go = new GameObject("ToDestroy");
            go.CDestroy();
            Assert.IsTrue(go == null);
        }

        [Test]
        public void CDestroyImmediate_DestroysGameObject()
        {
            var go = new GameObject("ToDestroyImmediate");
            go.CDestroyImmediate();
            Assert.IsTrue(go == null);
        }

        [Test]
        public void CDestroyImmediate_WithNull_DoesNotThrow()
        {
            Object obj = null;
            Assert.DoesNotThrow(() => obj.CDestroyImmediate());
        }

        [Test]
        public void CDoIfNotNull_WithNonNull_InvokesAction()
        {
            var go = new GameObject("Test");
            bool invoked = false;
            go.CDoIfNotNull<GameObject>(g => invoked = true);
            Assert.IsTrue(invoked);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CDoIfNotNull_WithNull_DoesNotInvoke()
        {
            GameObject nullGo = null;
            bool invoked = false;
            nullGo.CDoIfNotNull<GameObject>(g => invoked = true);
            Assert.IsFalse(invoked);
        }

        [Test]
        public void CDoIfNotNull_WithNullAction_DoesNotThrow()
        {
            var go = new GameObject("Test");
            Assert.DoesNotThrow(() => go.CDoIfNotNull<GameObject>(null));
            Object.DestroyImmediate(go);
        }
    }
}
