using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace EnigmaCore
{
    public static class SceneExtensions
    {
        public static bool TryFindAtRoot<T>(this Scene scene, out T finding)
        {
            using var pooledObject = ListPool<GameObject>.Get(out var rootGameObjects);
            scene.GetRootGameObjects(rootGameObjects);

            for (var i = 0; i < rootGameObjects.Count; i++)
            {
                if (rootGameObjects[i].TryGetComponent<T>(out finding))
                {
                    return true;
                }
            }

            finding = default;
            return false;
        }

        #region <<---------- Editor ---------->>

        public static void EditorSetSceneExpanded(this Scene scene, bool expand) {
            #if UNITY_EDITOR
            try {
                if (PrefabStageUtility.GetCurrentPrefabStage()) return;
                if (!scene.IsValid()) {
                    Debug.LogWarning($"Cannot set expanded state of an invalid scene: {scene}");
                    return;
                }
                foreach (var window in Resources.FindObjectsOfTypeAll<SearchableEditorWindow>()) {
                    if (window != null && window.GetType().Name != "SceneHierarchyWindow") continue;

                    var method = window.GetType().GetMethod("SetExpandedRecursive",
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance, null,
                        new[] { typeof(int), typeof(bool) }, null);

                    if (method == null) {
                        Debug.LogError("Could not find method 'UnityEditor.SceneHierarchyWindow.SetExpandedRecursive(int, bool)'.");
                        return;
                    }

                    var field = scene.GetType().GetField("m_Handle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (field == null) {
                        Debug.LogError("Could not find field 'int UnityEngine.SceneManagement.Scene.m_Handle'.");
                        return;
                    }

                    var sceneHandle = field.GetValue(scene);
                    method.Invoke(window, new[] { sceneHandle, expand });
                }

            } catch (Exception e) {
                Debug.LogError(e);
            }
            #endif
        }

        #endregion <<---------- Editor ---------->>
    }
}