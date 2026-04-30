using UnityEditor;
using UnityEngine;

namespace NCDK.Editor
{
    [CustomPropertyDrawer(typeof(RequiredListLengthAttribute))]
    public class RequiredListLengthAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RequiredListLengthAttribute attr = (RequiredListLengthAttribute)attribute;
            
            bool isInvalid = false;
            
            // isArray covers List<> and array []
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                if (property.arraySize < attr.MinLength || property.arraySize > attr.MaxLength)
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
            else
            {
                EditorGUI.HelpBox(position, "RequiredListLengthAttribute works only on arrays/lists.", MessageType.Warning);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isArray || property.propertyType == SerializedPropertyType.String)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
