using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class TransformExtensionsTests
    {
        private GameObject _go;
        private Transform _transform;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("TestTransform");
            _transform = _go.transform;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        [Test]
        public void RotateTowardsDirection_WithZeroTimeScale_DoesNotRotate()
        {
            var original = _transform.rotation;
            _transform.RotateTowardsDirection(Vector3.forward, 90f, 0f);
            Assert.AreEqual(original, _transform.rotation);
        }

        [Test]
        public void RotateTowardsDirection_WithZeroDirection_DoesNotRotate()
        {
            var original = _transform.rotation;
            _transform.RotateTowardsDirection(Vector3.zero, 90f, 1f);
            Assert.AreEqual(original, _transform.rotation);
        }

        [Test]
        public void GetParentOrSelf_WithNoParent_ReturnsSelf()
        {
            Assert.AreEqual(_transform, _transform.GetParentOrSelf());
        }

        [Test]
        public void GetParentOrSelf_WithParent_ReturnsParent()
        {
            var parent = new GameObject("Parent").transform;
            _transform.SetParent(parent);
            Assert.AreEqual(parent, _transform.GetParentOrSelf());
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void ResetTransform_SetsLocalPositionToZero()
        {
            _transform.localPosition = new Vector3(5, 10, 15);
            _transform.ResetTransform();
            Assert.AreEqual(Vector3.zero, _transform.localPosition);
        }

        [Test]
        public void ResetTransform_SetsLocalRotationToIdentity()
        {
            _transform.localRotation = Quaternion.Euler(45, 30, 60);
            _transform.ResetTransform();
            Assert.AreEqual(Quaternion.identity, _transform.localRotation);
        }

        [Test]
        public void ResetTransform_SetsLocalScaleToOne()
        {
            _transform.localScale = new Vector3(2, 3, 4);
            _transform.ResetTransform();
            Assert.AreEqual(Vector3.one, _transform.localScale);
        }

        [Test]
        public void DestroyAllChildren_WithNoChildren_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _transform.DestroyAllChildren());
        }

        [Test]
        public void DestroyAllChildren_WithNullTransform_DoesNotThrow()
        {
            Transform nullTransform = null;
            Assert.DoesNotThrow(() => nullTransform.DestroyAllChildren());
        }

        [Test]
        public void UnparentAllChildren_RemovesParent()
        {
            var child = new GameObject("Child");
            child.transform.SetParent(_transform);

            _transform.UnparentAllChildren();

            Assert.AreEqual(0, _transform.childCount);
            Assert.IsNull(child.transform.parent);
            Object.DestroyImmediate(child);
        }

        [Test]
        public void GetRelative3dPlanarDirection_WithForwardInput_ReturnsForward()
        {
            var dir = _transform.GetRelative3dPlanarDirection(new Vector3(0, 0, 1));
            Assert.AreEqual(_transform.forward.normalized, dir.normalized);
        }

        [Test]
        public void GetRelative3dPlanarDirection_WithRightInput_ReturnsRight()
        {
            var dir = _transform.GetRelative3dPlanarDirection(new Vector3(1, 0, 0));
            Assert.AreEqual(_transform.right.normalized, dir.normalized);
        }
    }
}
