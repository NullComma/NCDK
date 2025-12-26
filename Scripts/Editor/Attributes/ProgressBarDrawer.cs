#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace EnigmaCore
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use ProgressBar with float or int.");
                return;
            }

            var attr = (ProgressBarAttribute)attribute;
            float min = attr.Min;
            float max = GetMaxValue(property, attr);
            float current = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
            
            // Verifica se tem ReadOnly no campo
            bool isReadOnly = fieldInfo.GetCustomAttribute(typeof(ReadOnlyAttribute)) != null;

            // Draw Label
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            // Draw Bar using Helper
            Rect barRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);
            
            float newValue = EnigmaEditorGUI.DrawProgressBar(barRect, current, min, max, attr, isReadOnly);

            if (!isReadOnly && property.propertyType == SerializedPropertyType.Float)
                property.floatValue = newValue;
            else if (!isReadOnly && property.propertyType == SerializedPropertyType.Integer)
                property.intValue = Mathf.RoundToInt(newValue);
        }

        private float GetMaxValue(SerializedProperty property, ProgressBarAttribute attr)
        {
            if (string.IsNullOrEmpty(attr.MaxMemberName)) return attr.Max;
            
            object target = property.serializedObject.targetObject;
            // Simplificado para brevidade, idealmente usa reflection recursiva se necessário
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            
            var field = target.GetType().GetField(attr.MaxMemberName, flags);
            if (field != null) return System.Convert.ToSingle(field.GetValue(target));
            
            var prop = target.GetType().GetProperty(attr.MaxMemberName, flags);
            if (prop != null) return System.Convert.ToSingle(prop.GetValue(target));

            return attr.Max;
        }
    }
}
#endif