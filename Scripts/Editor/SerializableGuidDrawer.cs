using System;
using UnityEditor;
using UnityEngine;

namespace EnigmaCore.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 25f; 
        private const float Spacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 1. Draw the Label
            Rect labelRect = position;
            labelRect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(labelRect, label);

            // 2. Calculate Rects
            Rect contentRect = position;
            contentRect.x += labelRect.width;
            contentRect.width -= labelRect.width;

            // Define area for the Text and the Button
            Rect textFieldRect = new Rect(contentRect.x, contentRect.y, contentRect.width - ButtonWidth - Spacing, contentRect.height);
            Rect buttonRect = new Rect(contentRect.x + contentRect.width - ButtonWidth, contentRect.y, ButtonWidth, contentRect.height);

            // 3. Get Properties
            SerializedProperty p1 = property.FindPropertyRelative("Part1");
            SerializedProperty p2 = property.FindPropertyRelative("Part2");
            SerializedProperty p3 = property.FindPropertyRelative("Part3");
            SerializedProperty p4 = property.FindPropertyRelative("Part4");

            // 4. Construct GUID String
            var tempGuid = new SerializableGuid(
                (uint)p1.longValue, 
                (uint)p2.longValue,
                (uint)p3.longValue,
                (uint)p4.longValue
            );
            string guidString = tempGuid.ToGuid().ToString();

            // 5. Draw the GUID as a Selectable Label with TextField styling
            // This allows selecting/copying text but prevents editing, fixing the previous issue.
            EditorGUI.SelectableLabel(textFieldRect, guidString, EditorStyles.textField);

            // 6. Regenerate Button
            var icon = EditorGUIUtility.IconContent("d_Refresh");
            var buttonContent = icon.image ? icon : new GUIContent("R", "Regenerate GUID");
            if (GUI.Button(buttonRect, buttonContent))
            {
                RegenerateGuid(property, p1, p2, p3, p4);
            }

            // 7. Context Menu (Right Click)
            // Still useful if the user wants to regenerate via right-click or copy without selecting
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1 && textFieldRect.Contains(e.mousePosition))
            {
                ShowContextMenu(property, p1, p2, p3, p4, guidString);
                e.Use();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Ensure standard height to avoid layout glitches with SelectableLabel
            return EditorGUIUtility.singleLineHeight;
        }

        private void ShowContextMenu(SerializedProperty rootProperty, SerializedProperty p1, SerializedProperty p2, SerializedProperty p3, SerializedProperty p4, string currentString)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy GUID"), false, () => 
            {
                EditorGUIUtility.systemCopyBuffer = currentString;
            });
            
            menu.AddItem(new GUIContent("Regenerate GUID"), false, () => RegenerateGuid(rootProperty, p1, p2, p3, p4));
            
            menu.AddItem(new GUIContent("Reset to Empty"), false, () => 
            {
                p1.longValue = 0;
                p2.longValue = 0;
                p3.longValue = 0;
                p4.longValue = 0;
                rootProperty.serializedObject.ApplyModifiedProperties();
            });
            
            menu.ShowAsContext();
        }

        private void RegenerateGuid(SerializedProperty rootProperty, SerializedProperty p1, SerializedProperty p2, SerializedProperty p3, SerializedProperty p4)
        {
            if (!EditorUtility.DisplayDialog("Regenerate GUID", 
                "Are you sure you want to regenerate this ID? This may break existing references.", "Regenerate", "Cancel")) 
            {
                return;
            }

            byte[] bytes = Guid.NewGuid().ToByteArray();

            p1.longValue = BitConverter.ToUInt32(bytes, 0);
            p2.longValue = BitConverter.ToUInt32(bytes, 4);
            p3.longValue = BitConverter.ToUInt32(bytes, 8);
            p4.longValue = BitConverter.ToUInt32(bytes, 12);

            rootProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}