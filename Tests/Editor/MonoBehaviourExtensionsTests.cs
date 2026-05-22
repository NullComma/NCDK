using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class MonoBehaviourExtensionsTests
    {
        private GameObject _go;
        private MonoBehaviour _mb;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("TestMB");
            _mb = _go.AddComponent<MonoBehaviourTestComponent>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        [Test]
        public void CStopCoroutine_WithNullCoroutine_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _mb.CStopCoroutine(null));
        }

        [Test]
        public void CStopCoroutine_WithNullMB_DoesNotThrow()
        {
            MonoBehaviour nullMb = null;
            Assert.DoesNotThrow(() => nullMb.CStopCoroutine(null));
        }

        [Test]
        public void CResolveComponentFromChildrenIfNull_WithNullRef_FindsComponent()
        {
            BoxCollider result = null;
            _mb.CResolveComponentFromChildrenIfNull(ref result);
            Assert.IsNull(result, "No child component should exist initially");
        }

        [Test]
        public void CResolveComponentFromChildrenIfNull_WithChildComponent_FindsIt()
        {
            var child = new GameObject("Child");
            child.transform.SetParent(_go.transform);
            child.AddComponent<BoxCollider>();

            BoxCollider result = null;
            _mb.CResolveComponentFromChildrenIfNull(ref result);
            Assert.IsNotNull(result);
        }

        [Test]
        public void CResolveComponentFromChildrenIfNull_WithExistingRef_DoesNotChange()
        {
            var child = new GameObject("Child");
            child.transform.SetParent(_go.transform);
            var childRb = child.AddComponent<Rigidbody>();

            BoxCollider existing = _go.AddComponent<BoxCollider>();
            _mb.CResolveComponentFromChildrenIfNull(ref existing);

            Assert.IsNotNull(existing);
        }

        [Test]
        public void CSetNameIfOnlyComponent_WithSingleComponent_SetsName()
        {
            var go = new GameObject("Original");
            var mb = go.AddComponent<MonoBehaviourTestComponent>();

            mb.CSetNameIfOnlyComponent("Renamed");

            Assert.AreEqual("Renamed", mb.name);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CSetNameIfOnlyComponent_WithMultipleComponents_DoesNotRename()
        {
            var go = new GameObject("Original");
            var mb = go.AddComponent<MonoBehaviourTestComponent>();
            go.AddComponent<BoxCollider>();

            mb.CSetNameIfOnlyComponent("Renamed");

            Assert.AreEqual("Original", mb.name);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CSetNameIfOnlyComponent_WithNull_DoesNotThrow()
        {
            MonoBehaviour nullMb = null;
            Assert.DoesNotThrow(() => nullMb.CSetNameIfOnlyComponent("test"));
        }

        class MonoBehaviourTestComponent : MonoBehaviour { }
    }
}
