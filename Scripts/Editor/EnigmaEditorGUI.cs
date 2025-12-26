using UnityEditor;
using UnityEngine;

namespace EnigmaCore
{
    public static class EnigmaEditorGUI
    {
        public static float DrawProgressBar(Rect position, float current, float min, float max, ProgressBarAttribute attr, bool isReadOnly)
        {
            // Draw Label and background
            Rect barRect = position;
            
            // Interaction
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            float newValue = current;

            if (attr.IsEditable && !isReadOnly)
            {
                newValue = HandleBarInput(barRect, controlID, min, max, current);
            }

            // Draw Background
            EditorGUI.DrawRect(barRect, new Color(0.1f, 0.1f, 0.1f));

            // Draw Fill
            float t = Mathf.InverseLerp(min, max, newValue);
            Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * t, barRect.height);
            EditorGUI.DrawRect(fillRect, attr.Color);

            // Draw Text
            string text = $"{newValue:F2} / {max:F2}";
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
            EditorGUI.LabelField(barRect, text, centeredStyle);

            return newValue;
        }

        private static float HandleBarInput(Rect rect, int id, float min, float max, float current)
        {
            Event evt = Event.current;
            float result = current;

            if (evt.type == EventType.MouseDown && rect.Contains(evt.mousePosition))
            {
                GUIUtility.hotControl = id;
                result = CalculateValue(evt.mousePosition, rect, min, max);
                evt.Use();
            }
            else if (evt.type == EventType.MouseDrag && GUIUtility.hotControl == id)
            {
                result = CalculateValue(evt.mousePosition, rect, min, max);
                evt.Use();
            }
            else if (evt.type == EventType.MouseUp && GUIUtility.hotControl == id)
            {
                GUIUtility.hotControl = 0;
                evt.Use();
            }

            return result;
        }

        private static float CalculateValue(Vector2 mousePos, Rect rect, float min, float max)
        {
            float relativeX = mousePos.x - rect.x;
            float percentage = Mathf.Clamp01(relativeX / rect.width);
            return Mathf.Lerp(min, max, percentage);
        }
    }
}
