using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NCDK
{
    public static class ScriptableObjectExtensions
    {
        #if UNITY_EDITOR
        public static T EditorCreateInResourcesFolder<T>() where T : ScriptableObject
        {
            var so = ScriptableObject.CreateInstance<T>();
            var path = "Assets/Resources";
            
            // Unity 2023.1+ uses AssetPathExists, older versions use IsValidFolder
            #if UNITY_2023_1_OR_NEWER
            if(!AssetDatabase.AssetPathExists(path)) AssetDatabase.CreateFolder("Assets", "Resources");
            #else
            if(!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder("Assets", "Resources");
            #endif
            
            AssetDatabase.CreateAsset(so, $"{path}/{nameof(T)}.asset");
			return so as T;
        }
        #endif
    }
}