using NUnit.Framework;
using UnityEngine;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class ColliderExtensionsTests
    {
        [Test]
        public void CCloneColliderAsChild_WithNull_DoesNotThrow()
        {
            Collider nullCol = null;
            Assert.DoesNotThrow(() => nullCol.CCloneColliderAsChild(0));
        }

        [Test]
        public void CCloneColliderAsChild_WithBoxCollider_CreatesClone()
        {
            var go = new GameObject("Box");
            var box = go.AddComponent<BoxCollider>();
            box.center = new Vector3(1, 2, 3);
            box.size = new Vector3(4, 5, 6);

            box.CCloneColliderAsChild(10);

            Assert.AreEqual(1, go.transform.childCount);
            var cloneGo = go.transform.GetChild(0).gameObject;
            var clone = cloneGo.GetComponent<BoxCollider>();
            Assert.IsNotNull(clone);
            Assert.AreEqual(box.center, clone.center);
            Assert.AreEqual(box.size, clone.size);
            Assert.AreEqual(10, cloneGo.layer);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CCloneColliderAsChild_WithSphereCollider_CreatesClone()
        {
            var go = new GameObject("Sphere");
            var sphere = go.AddComponent<SphereCollider>();
            sphere.center = new Vector3(1, 2, 3);
            sphere.radius = 5f;

            sphere.CCloneColliderAsChild(5);

            Assert.AreEqual(1, go.transform.childCount);
            var clone = go.transform.GetChild(0).GetComponent<SphereCollider>();
            Assert.IsNotNull(clone);
            Assert.AreEqual(sphere.center, clone.center);
            Assert.AreEqual(sphere.radius, clone.radius);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void CCloneColliderAsChild_WithCapsuleCollider_CreatesClone()
        {
            var go = new GameObject("Capsule");
            var capsule = go.AddComponent<CapsuleCollider>();
            capsule.center = new Vector3(1, 2, 3);
            capsule.radius = 4f;
            capsule.height = 8f;

            capsule.CCloneColliderAsChild(15);

            Assert.AreEqual(1, go.transform.childCount);
            var clone = go.transform.GetChild(0).GetComponent<CapsuleCollider>();
            Assert.IsNotNull(clone);
            Assert.AreEqual(capsule.center, clone.center);
            Assert.AreEqual(capsule.radius, clone.radius);
            Assert.AreEqual(capsule.height, clone.height);
            Object.DestroyImmediate(go);
        }
    }
}
