using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class ComponentExtensionsTests
    {
        private GameObject _go;
        private BoxCollider _collider;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("Test");
            _collider = _go.AddComponent<BoxCollider>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        [Test]
        public void GetOrAddComponent_WithExisting_ReturnsExisting()
        {
            var result = _collider.GetOrAddComponent<BoxCollider>();
            Assert.AreEqual(_collider, result);
        }

        [Test]
        public void GetOrAddComponent_WithoutExisting_AddsNew()
        {
            var result = _collider.GetOrAddComponent<Rigidbody>();
            Assert.IsNotNull(result);
            Assert.IsNotNull(_go.GetComponent<Rigidbody>());
        }

        [Test]
        public void GetOrAddComponent_WithNullComponent_ReturnsDefault()
        {
            LogAssert.Expect(LogType.Error, "Cant add component to a null component!");
            Component nullComp = null;
            Assert.AreEqual(default, nullComp.GetOrAddComponent<BoxCollider>());
        }

        [Test]
        public void DestroyGameObject_DestroysGameObject()
        {
            var go = new GameObject("ToDestroy");
            var comp = go.AddComponent<BoxCollider>();
            comp.DestroyGameObject(0f);
            Assert.IsTrue(go == null, "GameObject should be destroyed");
        }

        [Test]
        public void DestroyGameObject_WithNullComponent_DoesNotThrow()
        {
            Component nullComp = null;
            Assert.DoesNotThrow(() => nullComp.DestroyGameObject(0f));
        }

        [Test]
        public void AssertIfNull_WithNonNull_ReturnsFalse()
        {
            bool isNull = _collider.AssertIfNull("test");
            Assert.IsFalse(isNull);
        }

        [Test]
        public void AssertIfNull_WithNull_ReturnsTrue()
        {
            LogAssert.Expect(LogType.Error, "<b>Assert</b>: Component is null (test)");
            BoxCollider nullCol = null;
            bool isNull = nullCol.AssertIfNull("test");
            Assert.IsTrue(isNull);
        }

        [Test]
        public void ThrowIfNull_WithNonNull_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _collider.ThrowIfNull("test"));
        }

        [Test]
        public void ThrowIfNull_WithNull_ThrowsNullReferenceException()
        {
            LogAssert.Expect(LogType.Exception, "NullReferenceException: Component is null (test)");
            BoxCollider nullCol = null;
            Assert.Throws<System.NullReferenceException>(() => nullCol.ThrowIfNull("test"));
        }

        [Test]
        public void GetComponentInChildrenOrInParent_InChildren_FindsChild()
        {
            var child = new GameObject("Child");
            child.transform.SetParent(_go.transform);
            var childRb = child.AddComponent<Rigidbody>();

            var result = _collider.GetComponentInChildrenOrInParent<Rigidbody>();
            Assert.AreEqual(childRb, result);
        }

        [Test]
        public void GetComponentInParentOrInChildren_InChildren_FindsChild()
        {
            var child = new GameObject("Child");
            child.transform.SetParent(_go.transform);
            var childRb = child.AddComponent<Rigidbody>();

            var result = _collider.GetComponentInParentOrInChildren<Rigidbody>();
            Assert.AreEqual(childRb, result);
        }
    }
}
