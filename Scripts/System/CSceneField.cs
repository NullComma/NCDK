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
        private string sceneName = "";
#pragma warning restore 414

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Operator Overload ---------->>

        /// <summary>
        /// Allows this object to be used where a string is expected.
        /// </summary>
        public static implicit operator string(CSceneField sceneField) {
            return sceneField?.ToString() ?? "";
        }

          public static bool operator ==(CSceneField a, CSceneField b) {
            // Handle null comparisons
            if (ReferenceEquals(a, null)) {
                return ReferenceEquals(b, null);
            }
            return a.Equals(b);
        }
        
        public static bool operator !=(CSceneField a, CSceneField b) => !(a == b);

        #endregion <<---------- Operator Overload ---------->>

        #region <<---------- Overrides ---------->>

        /// <summary>
        /// Returns the string representation of the scene.
        /// In the editor, this is the asset path. In a build, it's the scene name.
        /// </summary>
        public override string ToString() {
            #if UNITY_EDITOR
            return sceneAsset != null ? AssetDatabase.GetAssetPath(sceneAsset) : "[No Scene]";
            #else
            return sceneName;
            #endif
        }

        public override bool Equals(object obj) {
            if (obj is CSceneField other) {
                return this.sceneName == other.sceneName;
            }
            return false;
        }

        public override int GetHashCode() {
            return sceneName.GetHashCode();
        }

        #endregion <<---------- Overrides ---------->>

        #region <<---------- Serialization ---------->>

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (sceneAsset != null) {
                // We must save the scene name, not the path, for it to work in a build.
                sceneName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(sceneAsset));
            }
            else {
                sceneName = "";
            }
#endif
        }

        public void OnAfterDeserialize() { }

        #endregion <<---------- Serialization ---------->>




        #region <<---------- Public Methods ---------->>

        public void LoadScene()
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAdditive()
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        #endregion <<---------- Public Methods ---------->>

    }
}