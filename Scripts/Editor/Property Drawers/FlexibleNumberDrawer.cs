using UnityEditor;
using UnityEngine;

namespace NullCore.Editor
{
    [CustomPropertyDrawer(typeof(FlexibleFloat))]
    [CustomPropertyDrawer(typeof(FlexibleInt))]
    public class FlexibleNumberDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            float modeWidth = 65;
            float spacing = 5;
            float valueWidth = position.width - modeWidth - spacing;

            Rect modeRect = new Rect(position.x, position.y, modeWidth, position.height);
            Rect valueRect = new Rect(position.x + modeWidth + spacing, position.y, valueWidth, position.height);

            // Get properties
            SerializedProperty modeProp = property.FindPropertyRelative("_mode");
            SerializedProperty constantProp = property.FindPropertyRelative("_value");
            SerializedProperty minProp = property.FindPropertyRelative("_min");
            SerializedProperty maxProp = property.FindPropertyRelative("_max");

            // Draw mode
            EditorGUI.PropertyField(modeRect, modeProp, GUIContent.none);

            // Draw values based on mode
            if (modeProp.enumValueIndex == 0) // Constant
            {
                EditorGUI.PropertyField(valueRect, constantProp, GUIContent.none);
            }
            else // Range
            {
                float halfWidth = (valueWidth - spacing) / 2;
                Rect minRect = new Rect(valueRect.x, valueRect.y, halfWidth, valueRect.height);
                Rect maxRect = new Rect(valueRect.x + halfWidth + spacing, valueRect.y, halfWidth, valueRect.height);

                // Use a smaller label for min/max or just the fields
                EditorGUI.PropertyField(minRect, minProp, GUIContent.none);
                EditorGUI.PropertyField(maxRect, maxProp, GUIContent.none);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
