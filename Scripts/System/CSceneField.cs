using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnigmaCore
{
    [Serializable]
    public class CSceneField : ISerializationCallbackReceiver
    {
        #region <<---------- Properties and Fields ---------->>

#if UNITY_EDITOR
        [SerializeField]
        SceneAsset sceneAsset;
#endif

#pragma warning disable 414
        [SerializeField, HideInInspector]
        string sceneName = "";

        [SerializeField, HideInInspector]
        string scenePath = "";
#pragma warning restore 414

        // Public accessors for validation logic
        public string SceneName => sceneName;
        public string ScenePath => scenePath;

        #endregion <<---------- Properties and Fields ---------->>


        #region <<---------- Constructors ---------->>

        /// <summary>
        /// Creates a CSceneField. Ideally, provide the full Asset Path for Bundle compatibility.
        /// </summary>
        /// <param name="sceneIdentifier">Scene name or "Assets/..." path.</param>
        public CSceneField(string sceneIdentifier)
        {
            if (string.IsNullOrEmpty(sceneIdentifier)) return;

            // Normalize path separators just in case
            string cleanId = sceneIdentifier.Replace("\\", "/");

#if UNITY_EDITOR
            // 1. Try resolving via AssetDatabase to ensure data integrity
            sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(cleanId);

            // 2. If not a path, search by name
            if (sceneAsset == null && !cleanId.Contains("/"))
            {
                string[] guids = AssetDatabase.FindAssets($"t:Scene {cleanId}");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                }
            }

            if (sceneAsset != null)
            {
                sceneName = sceneAsset.name;
                scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            }
            else
            {
                // Fallback: Manually parse string if not found in Editor (simulating Runtime creation)
                SetRuntimeData(cleanId);
            }
#else
            // Runtime logic
            SetRuntimeData(cleanId);
#endif
        }

#if UNITY_EDITOR
        public CSceneField(SceneAsset scene)
        {
            sceneAsset = scene;
            OnBeforeSerialize(); // Force data sync
        }
#endif

        // Helper to populate data without Editor API
        void SetRuntimeData(string identifier)
        {
            // If it looks like a path
            if (identifier.Contains("/"))
            {
                scenePath = identifier;
                sceneName = Path.GetFileNameWithoutExtension(identifier);
            }
            else
            {
                sceneName = identifier;
                scenePath = ""; // Path unknown, relies on Name-based loading
            }
        }

        #endregion <<---------- Constructors ---------->>


        #region <<---------- Operator Overload ---------->>

        public static implicit operator string(CSceneField sceneField) => sceneField?.ToString() ?? "";

        public static bool operator ==(CSceneField a, CSceneField b)
        {
            if (ReferenceEquals(a, null)) return ReferenceEquals(b, null);
            return a.Equals(b);
        }

        public static bool operator !=(CSceneField a, CSceneField b) => !(a == b);

        #endregion <<---------- Operator Overload ---------->>


        #region <<---------- Overrides ---------->>

        public override string ToString()
        {
            // Validates which data is stronger. Path is always more precise.
            return !string.IsNullOrEmpty(scenePath) ? scenePath : sceneName;
        }

        public override bool Equals(object obj)
        {
            if (obj is CSceneField other)
            {
                // If both have paths, compare paths (more accurate for Bundles)
                if (!string.IsNullOrEmpty(this.scenePath) && !string.IsNullOrEmpty(other.scenePath))
                    return this.scenePath == other.scenePath;
                
                // Fallback to name comparison
                return this.sceneName == other.sceneName;
            }
            return false;
        }

        public override int GetHashCode() => scenePath.GetHashCode();

        #endregion <<---------- Overrides ---------->>


        #region <<---------- Serialization ---------->>

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (sceneAsset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(sceneAsset);
                sceneName = Path.GetFileNameWithoutExtension(assetPath);
                scenePath = assetPath;
            }
#endif
        }

        public void OnAfterDeserialize() { }

        #endregion <<---------- Serialization ---------->>


        #region <<---------- Public Methods ---------->>

        /// <summary>
        /// Loads the scene. Prioritizes Path (AssetBundles friendly), falls back to Name.
        /// </summary>
        public void LoadScene()
        {
            if (CanLoadByPath())
                SceneManager.LoadScene(scenePath);
            else if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAdditive()
        {
            if (CanLoadByPath())
                SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);
            else if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Checks if the scene is valid in Build Settings.
        /// Note: For loaded Asset Bundles, this check might return false even if valid, 
        /// so rely on LoadScene throwing/logging if you are strictly using Bundles.
        /// </summary>
        public bool IsInBuildSettings()
        {
            // -1 means not found in Build Settings
            if (CanLoadByPath())
                return SceneUtility.GetBuildIndexByScenePath(scenePath) != -1;
            
            return false;
        }

        #endregion <<---------- Public Methods ---------->>


        #region <<---------- Internal Helpers ---------->>

        bool CanLoadByPath()
        {
            // We only use path if it looks like a Unity path ("Assets/...")
            // or if we are sure it came from a Bundle logic.
            return !string.IsNullOrEmpty(scenePath) && scenePath.Contains("/");
        }

        #endregion <<---------- Internal Helpers ---------->>
    }
}