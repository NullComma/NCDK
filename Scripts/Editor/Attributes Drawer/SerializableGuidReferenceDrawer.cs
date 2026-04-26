#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NullCore.Editor
{
    /// <summary>
    /// Custom drawer for SerializableGuidReference that uses an AdvancedDropdown.
    /// Heavily optimized to avoid finding objects during OnGUI calls.
    /// Click on the property label to select and ping the referenced object in the scene.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableGuidReference))]
    public class SerializableGuidReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty valueProp = property.FindPropertyRelative("Value");
            SerializedProperty p1 = valueProp.FindPropertyRelative("Part1");
            SerializedProperty p2 = valueProp.FindPropertyRelative("Part2");
            SerializedProperty p3 = valueProp.FindPropertyRelative("Part3");
            SerializedProperty p4 = valueProp.FindPropertyRelative("Part4");

            SerializableGuid currentGuid = new SerializableGuid(
                (uint)p1.longValue,
                (uint)p2.longValue,
                (uint)p3.longValue,
                (uint)p4.longValue);

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && labelRect.Contains(currentEvent.mousePosition))
            {
                if (currentGuid != SerializableGuid.Empty && IdentifiableRegistry.TryGetObject(currentGuid, out var targetObject))
                {
                    if (targetObject != null)
                    {
                        EditorGUIUtility.PingObject(targetObject);
                        Selection.activeObject = targetObject;
                        currentEvent.Use();
                    }
                }
            }

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            string buttonText = "None";
            if (currentGuid != SerializableGuid.Empty)
            {
                if (IdentifiableRegistry.TryGetObject(currentGuid, out var obj) && obj != null)
                {
                    buttonText = IdentifiableRegistry.GetObjectPath(obj);
                }
                else
                {
                    buttonText = "ID Assigned (Not Found)";
                }
            }

            if (EditorGUI.DropdownButton(position, new GUIContent(buttonText), FocusType.Keyboard))
            {
                IdentifiableDropdown dropdown = new IdentifiableDropdown(new AdvancedDropdownState(), valueProp, currentGuid);
                dropdown.Show(position);
            }

            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// Modern searchable dropdown implementation for selecting IdentifiableMonoBehaviours.
    /// </summary>
    public class IdentifiableDropdown : AdvancedDropdown
    {
        [System.NonSerialized]
        SerializedProperty _valueProp;

        public IdentifiableDropdown(AdvancedDropdownState state, SerializedProperty property, SerializableGuid current) : base(state)
        {
            _valueProp = property;
            minimumSize = new Vector2(250, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Identifiable Objects");

            AdvancedDropdownItem noneItem = new AdvancedDropdownItem("None");
            noneItem.id = 0;
            root.AddChild(noneItem);

            var allRegistered = IdentifiableRegistry.GetAllRegistered();

            foreach (var kvp in allRegistered)
            {
                SerializableGuid guid = kvp.Key;
                UnityEngine.Object obj = kvp.Value.FirstOrDefault();
                if (obj == null) continue;

                string groupName = "Assets";
                if (obj is Component comp && comp.gameObject.scene.IsValid()) groupName = comp.gameObject.scene.name;
                else if (obj is GameObject go && go.scene.IsValid()) groupName = go.scene.name;

                AdvancedDropdownItem sceneGroup = GetOrAddGroup(root, groupName);
                IdentifiableDropdownItem item = new IdentifiableDropdownItem(obj.name, guid);
                sceneGroup.AddChild(item);
            }

            return root;
        }

        AdvancedDropdownItem GetOrAddGroup(AdvancedDropdownItem root, string groupName)
        {
            foreach (AdvancedDropdownItem child in root.children)
            {
                if (child.name == groupName) return child;
            }

            AdvancedDropdownItem newGroup = new AdvancedDropdownItem(groupName);
            root.AddChild(newGroup);
            return newGroup;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item.name == "None")
            {
                ApplyGuid(SerializableGuid.Empty);
                return;
            }

            if (item is IdentifiableDropdownItem idItem)
            {
                ApplyGuid(idItem.Guid);
            }
        }

        void ApplyGuid(SerializableGuid guid)
        {
            _valueProp.serializedObject.Update();

            SerializedProperty p1 = _valueProp.FindPropertyRelative("Part1");
            SerializedProperty p2 = _valueProp.FindPropertyRelative("Part2");
            SerializedProperty p3 = _valueProp.FindPropertyRelative("Part3");
            SerializedProperty p4 = _valueProp.FindPropertyRelative("Part4");

            p1.longValue = guid.Part1;
            p2.longValue = guid.Part2;
            p3.longValue = guid.Part3;
            p4.longValue = guid.Part4;

            _valueProp.serializedObject.ApplyModifiedProperties();
            IdentifiableRegistry.MarkDirty();
        }
    }

    /// <summary>
    /// Holds the GUID data within the dropdown structure.
    /// </summary>
    public class IdentifiableDropdownItem : AdvancedDropdownItem
    {
        [System.NonSerialized]
        SerializableGuid _guid;

        public SerializableGuid Guid => _guid;

        public IdentifiableDropdownItem(string name, SerializableGuid guid) : base(name)
        {
            _guid = guid;
        }
    }
}
#endif