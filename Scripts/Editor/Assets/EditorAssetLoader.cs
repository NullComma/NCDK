using UnityEditor;
using UnityEngine;

namespace EnigmaCore.Editor
{
    public static class EditorAssetLoader
    {
        /// <summary>
        /// Loads an asset of type T from the specified path within the Assets folder.
        /// </summary>
        /// <param name="path">Relative to Assets folder.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadAssetFromPath<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            string fullPath = $"Assets/{path}";
            T asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);

            if(asset == null) 
                asset = AssetDatabase.LoadAssetAtPath<T>(fullPath + ".asset");
            
            if (asset == null)
            {
                Debug.LogError($"Failed to load asset at path: {fullPath}");
            }

            return asset;
#else
            // This will cause a compile error if used in runtime code
            throw new System.InvalidOperationException("EditorAssetLoader can only be used in Editor code.");
#endif
        }
    }
}