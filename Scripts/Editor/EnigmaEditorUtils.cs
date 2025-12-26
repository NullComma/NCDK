#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EnigmaCore
{
    public static class EnigmaEditorUtils
    {
        // --- LOGIC HELPERS ---

        public static bool EvaluateCondition(object target, string memberName)
        {
            if (string.IsNullOrEmpty(memberName) || target == null) return true;

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var type = target.GetType();

            // 1. Check Field
            var field = type.GetField(memberName, flags);
            if (field != null && field.FieldType == typeof(bool))
                return (bool)field.GetValue(target);

            // 2. Check Property
            var prop = type.GetProperty(memberName, flags);
            if (prop != null && prop.PropertyType == typeof(bool))
                return (bool)prop.GetValue(target);

            // 3. Check Method (parameterless)
            var method = type.GetMethod(memberName, flags, null, Type.EmptyTypes, null);
            if (method != null && method.ReturnType == typeof(bool))
                return (bool)method.Invoke(target, null);

            // Default to true if not found (or log warning)
            return true;
        }

        public static float GetValueFromMember(object target, string memberName, float defaultValue)
        {
            if (string.IsNullOrEmpty(memberName) || target == null) return defaultValue;
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var type = target.GetType();

            var field = type.GetField(memberName, flags);
            if (field != null) return Convert.ToSingle(field.GetValue(target));

            var prop = type.GetProperty(memberName, flags);
            if (prop != null && prop.CanRead) return Convert.ToSingle(prop.GetValue(target));

            return defaultValue;
        }

        public static (float min, float max) GetLimits(object target, float fixedMin, float fixedMax, string minMember, string maxMember)
        {
            float finalMin = string.IsNullOrEmpty(minMember) ? fixedMin : GetValueFromMember(target, minMember, fixedMin);
            float finalMax = string.IsNullOrEmpty(maxMember) ? fixedMax : GetValueFromMember(target, maxMember, fixedMax);
            return (finalMin, finalMax);
        }

        // --- DRAWING HELPERS ---

        public static void DrawInfoBox(string message, InfoMessageType type, bool condition)
        {
            if (!condition) return;

            MessageType unityType = MessageType.Info;
            switch (type)
            {
                case InfoMessageType.Warning: unityType = MessageType.Warning; break;
                case InfoMessageType.Error: unityType = MessageType.Error; break;
                case InfoMessageType.None: unityType = MessageType.None; break;
            }

            EditorGUILayout.HelpBox(message, unityType);
        }

        public static void DrawMinMaxSlider(Rect position, SerializedProperty property, MinMaxSliderAttribute attr)
        {
            // (Mesmo código da resposta anterior...)
             object target = property.serializedObject.targetObject;
            var (minLimit, maxLimit) = GetLimits(target, attr.Min, attr.Max, attr.MinMember, attr.MaxMember);

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                Vector2 val = property.vector2Value;
                float x = val.x, y = val.y;
                string labelText = $"{property.displayName} ({x:F1} - {y:F1})";
                EditorGUI.MinMaxSlider(position, new GUIContent(labelText), ref x, ref y, minLimit, maxLimit);
                property.vector2Value = new Vector2(x, y);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                Vector2Int val = property.vector2IntValue;
                float x = val.x, y = val.y;
                string labelText = $"{property.displayName} ({val.x} - {val.y})";
                EditorGUI.MinMaxSlider(position, new GUIContent(labelText), ref x, ref y, minLimit, maxLimit);
                property.vector2IntValue = new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(position, property, minLimit, maxLimit);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntSlider(position, property, (int)minLimit, (int)maxLimit);
            }
            else
            {
                EditorGUI.LabelField(position, property.displayName, "Use MinMaxSlider on float/int/Vector2");
            }
        }

        public static object DrawMinMaxSliderRaw(Rect position, string label, object value, Type type, object target, MinMaxSliderAttribute attr, bool isReadOnly)
        {
             // (Mesmo código da resposta anterior...)
             var (minLimit, maxLimit) = GetLimits(target, attr.Min, attr.Max, attr.MinMember, attr.MaxMember);
            
            EditorGUI.BeginDisabledGroup(isReadOnly);
            object result = value;

            if (type == typeof(Vector2))
            {
                Vector2 val = (Vector2)value;
                float x = val.x, y = val.y;
                string labelText = $"{label} ({x:F1} - {y:F1})";
                EditorGUI.MinMaxSlider(position, new GUIContent(labelText), ref x, ref y, minLimit, maxLimit);
                result = new Vector2(x, y);
            }
            else if (type == typeof(float))
            {
                float val = (float)value;
                result = EditorGUI.Slider(position, label, val, minLimit, maxLimit);
            }
            else if (type == typeof(int))
            {
                int val = (int)value;
                result = EditorGUI.IntSlider(position, label, val, (int)minLimit, (int)maxLimit);
            }
            EditorGUI.EndDisabledGroup();
            return result;
        }

        public static float DrawProgressBar(Rect position, float current, float min, float max, ProgressBarAttribute attr, bool isReadOnly)
        {
             // (Mesmo código da resposta anterior...)
             // Simplified Logic for brevity in this snippet, ensure you have the full draw logic
            Rect barRect = position;
            // Interaction logic...
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            // ... (Copy HandleBarInput from previous response)
            
            EditorGUI.DrawRect(barRect, new Color(0.1f, 0.1f, 0.1f));
            float t = Mathf.InverseLerp(min, max, current);
            Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * t, barRect.height);
            EditorGUI.DrawRect(fillRect, attr.Color);
            
            string text = $"{current:F2} / {max:F2}";
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
            EditorGUI.LabelField(barRect, text, centeredStyle);

            return current; // Return edited value if interaction logic added
        }
    }
}
#endif