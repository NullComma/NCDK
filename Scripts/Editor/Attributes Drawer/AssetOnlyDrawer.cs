using UnityEditor;
using UnityEngine;
using System.IO;

namespace EnigmaCore.Editor
{
    [CustomPropertyDrawer(typeof(AssetOnlyAttribute))]
    public class AssetOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.HelpBox(position, $"AssetOnlyAttribute only works on Object reference types.", MessageType.Error);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Use ObjectField with allowSceneObjects set to false to inherently restrict to project assets
            Object currentObject = property.objectReferenceValue;
            Object newObject = EditorGUI.ObjectField(position, label, currentObject, fieldInfo.FieldType, allowSceneObjects: false);

            if (newObject != currentObject)
            {
                if (newObject != null)
                {
                    string path = AssetDatabase.GetAssetPath(newObject);
                    bool isSceneAsset = path.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase);

                    if (isSceneAsset)
                    {
                        Debug.LogError($"AssetOnlyAttribute: Cannot assign scene asset '{newObject.name}' to field '{property.displayName}'. Only non-scene assets are permitted.", newObject);
                        // Block the assignment
                        newObject = currentObject; 
                    }
                    else if (string.IsNullOrEmpty(path) || !AssetDatabase.IsNativeAsset(newObject))
                    {
                        // Fallback check, though allowSceneObjects: false should handle runtime objects/components
                        Debug.LogError($"AssetOnlyAttribute: Assigned object '{newObject.name}' must be a saved asset, not a temporary object or a component.", newObject);
                        newObject = currentObject; 
                    }
                }

                property.objectReferenceValue = newObject;
            }

            EditorGUI.EndProperty();
        }
    }
}