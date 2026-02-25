using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;
#endif

namespace EnigmaCore
{
    /// <summary>
    /// Handles an object that should be disabled during Editor Play Mode 
    /// and completely stripped (removed) from the final Build.
    /// </summary>
    public class DestroyIfNotEditor : MonoBehaviour
    {
        private void Awake()
        {
            // Logic for Editor Play Mode only
            if (Application.isEditor)
            {
                Debug.Log($"[EnigmaCore] Disabling game object '{name}' because we are in Editor Play Mode.", this);
                this.gameObject.SetActive(false);
            }
        }
    }

    #if UNITY_EDITOR
    /// <summary>
    /// This class runs automatically during the Build Pipeline to strip the objects.
    /// It sits in the same file for convenience but only compiles in the Editor.
    /// </summary>
    public class DestroyIfNotEditorProcessor : IProcessSceneWithReport
    {
        public int callbackOrder => 0;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // report != null means we are making a Build (not just playing in editor)
            if (report == null) return;

            // Find all instances, including inactive ones
            var objectsToStrip = Resources.FindObjectsOfTypeAll<DestroyIfNotEditor>();

            if (objectsToStrip.Length > 0)
            {
                Debug.Log($"[EnigmaCore Build] Found {objectsToStrip.Length} objects to strip in scene '{scene.name}'.");

                foreach (var component in objectsToStrip)
                {
                    // Ensure we only destroy objects belonging to the scene currently being processed
                    if (component.gameObject.scene == scene)
                    {
                        Debug.Log($"[EnigmaCore Build] REMOVING object '{component.name}' from the build.");
                        Object.DestroyImmediate(component.gameObject);
                    }
                }
            }
        }
    }
    #endif
}