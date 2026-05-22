using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using NCDK;

namespace NCDK.Tests.Editor
{
    [TestFixture]
    public class SceneExtensionsTests
    {
        [Test]
        public void TryFindAtRoot_WithComponentAtRoot_ReturnsTrue()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var go = new GameObject("RootWithComponent");
            SceneManager.MoveGameObjectToScene(go, scene);
            var box = go.AddComponent<BoxCollider>();

            bool found = scene.TryFindAtRoot<BoxCollider>(out var result);

            Assert.IsTrue(found);
            Assert.AreEqual(box, result);
        }

        [Test]
        public void TryFindAtRoot_WithoutComponent_ReturnsFalse()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var go = new GameObject("RootWithoutComponent");
            SceneManager.MoveGameObjectToScene(go, scene);

            bool found = scene.TryFindAtRoot<BoxCollider>(out var result);

            Assert.IsFalse(found);
            Assert.IsNull(result);
        }

        [Test]
        public void TryFindAtRoot_WithComponentInChild_ReturnsFalse()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var parent = new GameObject("Parent");
            SceneManager.MoveGameObjectToScene(parent, scene);
            var child = new GameObject("Child");
            child.transform.SetParent(parent.transform);
            child.AddComponent<BoxCollider>();

            bool found = scene.TryFindAtRoot<BoxCollider>(out var result);

            Assert.IsFalse(found, "Should only find components on root objects, not children");
        }
    }
}
