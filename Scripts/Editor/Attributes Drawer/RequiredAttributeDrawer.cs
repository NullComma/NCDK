using UnityEditor;
using UnityEngine;

namespace NCDK.Editor
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RequiredAttribute attr = (RequiredAttribute)attribute;
            bool isInvalid = false;

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                isInvalid = true;
            }
            else if (property.propertyType == SerializedPropertyType.String && string.IsNullOrWhiteSpace(property.stringValue))
            {
                isInvalid = true;
            }

            Color originalColor = GUI.backgroundColor;
            if (isInvalid)
            {
                GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
            }

            EditorGUI.PropertyField(position, property, label, true);

            GUI.backgroundColor = originalColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
