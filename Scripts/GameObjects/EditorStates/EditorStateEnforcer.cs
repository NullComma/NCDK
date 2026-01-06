using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace EnigmaCore
{
    /// <summary>
    /// Base class for components that enforce a specific state (active/inactive) 
    /// on GameObjects or Components during Editor save events or play mode transitions.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class EditorStateEnforcer : MonoBehaviour
    {
        #if UNITY_EDITOR
        void Awake() => SignToEvents();
        void Reset() => SignToEvents();
        void OnValidate() => SignToEvents();

        private void SignToEvents()
        {
            if (Application.isPlaying) return;

            // Remove first to prevent double subscription
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorSceneManager.sceneSaving += OnSceneSaving;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            PrefabStage.prefabSaving -= OnPrefabSaving;
            PrefabStage.prefabSaving += OnPrefabSaving;
        }

        private void OnDestroy()
        {
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            PrefabStage.prefabSaving -= OnPrefabSaving;
        }

        private void OnPrefabSaving(GameObject go)
        {
            if (this == null) return;
            // Only enforce if this script is part of the prefab being saved
            if (go != this.gameObject && !this.transform.IsChildOf(go.transform)) return;
            
            EnforceState();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange s)
        {
            if (this == null) return;
            if (s == PlayModeStateChange.ExitingEditMode)
            {
                EnforceState();
            }
        }

        private void OnSceneSaving(Scene scene, string path)
        {
            if (this == null) return;
            EnforceState();
        }

        /// <summary>
        /// Logic to be executed when the state needs to be enforced (Save/Play).
        /// </summary>
        protected abstract void EnforceState();
        #endif
    }
}