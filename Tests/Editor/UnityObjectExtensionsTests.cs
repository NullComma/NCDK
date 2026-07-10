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
            Assert.DoesNotThrow(() => obj.Destroy());
        }

        [Test]
        public void CDestroy_DestroysGameObject()
        {
            var go = new GameObject("ToDestroy");
            go.Destroy();
            Assert.IsTrue(go == null);
        }

        [Test]
        public void CDestroyImmediate_DestroysGameObject()
        {
            var go = new GameObject("ToDestroyImmediate");
            go.DestroyImmediate();
            Assert.IsTrue(go == null);
        }

        [Test]
        public void CDestroyImmediate_WithNull_DoesNotThrow()
        {
            Object obj = null;
            Assert.DoesNotThrow(() => obj.DestroyImmediate());
        }

        [Test]
        public void CDoIfNotNull_WithNonNull_InvokesAction()
        {
            var go = new GameObject("Test");
            bool invoked = false;
            go.DoIfNotNull<GameObject>(g => invoked = true);
            Assert.IsTrue(invoked);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CDoIfNotNull_WithNull_DoesNotInvoke()
        {
            GameObject nullGo = null;
            bool invoked = false;
            nullGo.DoIfNotNull<GameObject>(g => invoked = true);
            Assert.IsFalse(invoked);
        }

        [Test]
        public void CDoIfNotNull_WithNullAction_DoesNotThrow()
        {
            var go = new GameObject("Test");
            Assert.DoesNotThrow(() => go.DoIfNotNull<GameObject>(null));
            Object.DestroyImmediate(go);
        }
    }
}
