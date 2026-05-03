#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NCDK.Editor
{
    /// <summary>
    /// Custom drawer for SerializableGuidReference that uses an AdvancedDropdown.
    /// Supports drag and drop of GameObjects and Components, including multi-object drops
    /// that can populate list/array elements sequentially.
    /// Click on the property label to select and ping the referenced object in the scene.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableGuidReference))]
    public class SerializableGuidReferenceDrawer : PropertyDrawer
    {
        const string ValueFieldName = nameof(SerializableGuidReference.Value);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty valueProp = property.FindPropertyRelative(ValueFieldName);
            if (valueProp == null)
            {
                EditorGUI.LabelField(position, label, new GUIContent("SerializableGuidReference.Value is missing."));
                EditorGUI.EndProperty();
                return;
            }

            SerializableGuid currentGuid = ReadGuid(valueProp);

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && labelRect.Contains(currentEvent.mousePosition))
            {
                if (currentGuid != SerializableGuid.Empty && IdentifiableRegistry.TryGetObject(currentGuid, out var targetObject) && targetObject != null)
                {
                    EditorGUIUtility.PingObject(targetObject);
                    Selection.activeObject = targetObject;
                    currentEvent.Use();
                }
            }

            Rect fieldRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Use the full row as the drop target so dragging onto the label area of list elements works too.
            HandleDragAndDrop(position, property, valueProp);

            string buttonText = GetDisplayText(currentGuid);
            if (EditorGUI.DropdownButton(fieldRect, new GUIContent(buttonText), FocusType.Keyboard))
            {
                IdentifiableDropdown dropdown = new IdentifiableDropdown(new AdvancedDropdownState(), valueProp);
                dropdown.Show(fieldRect);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;

        static SerializableGuid ReadGuid(SerializedProperty valueProp)
        {
            SerializedProperty p1 = valueProp.FindPropertyRelative(nameof(SerializableGuid.Part1));
            SerializedProperty p2 = valueProp.FindPropertyRelative(nameof(SerializableGuid.Part2));
            SerializedProperty p3 = valueProp.FindPropertyRelative(nameof(SerializableGuid.Part3));
            SerializedProperty p4 = valueProp.FindPropertyRelative(nameof(SerializableGuid.Part4));

            return new SerializableGuid(
                (uint)p1.longValue,
                (uint)p2.longValue,
                (uint)p3.longValue,
                (uint)p4.longValue);
        }

        static string GetDisplayText(SerializableGuid guid)
        {
            if (guid == SerializableGuid.Empty)
            {
                return "None";
            }

            if (IdentifiableRegistry.TryGetObject(guid, out var obj) && obj != null)
            {
                return IdentifiableRegistry.GetObjectPath(obj);
            }

            return "ID Assigned (Not Found)";
        }

        static void HandleDragAndDrop(Rect rect, SerializedProperty property, SerializedProperty valueProp)
        {
            Event currentEvent = Event.current;
            if (!rect.Contains(currentEvent.mousePosition))
            {
                return;
            }

            if (currentEvent.type != EventType.DragUpdated && currentEvent.type != EventType.DragPerform)
            {
                return;
            }

            List<SerializableGuid> droppedGuids = ResolveDraggedGuids(DragAndDrop.objectReferences);
            if (droppedGuids.Count == 0)
            {
                return;
            }

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (currentEvent.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                ApplyDraggedGuids(property, valueProp, droppedGuids);
            }

            currentEvent.Use();
        }

        static void ApplyDraggedGuids(SerializedProperty property, SerializedProperty valueProp, IReadOnlyList<SerializableGuid> droppedGuids)
        {
            if (property == null || valueProp == null || droppedGuids == null || droppedGuids.Count == 0)
            {
                return;
            }

            SerializedObject serializedObject = property.serializedObject;
            Undo.RecordObjects(serializedObject.targetObjects, "Assign Serializable Guid References");
            serializedObject.Update();

            if (TryGetArrayElementContext(property, out SerializedProperty arrayProperty, out int elementIndex))
            {
                string arrayPath = arrayProperty.propertyPath;
                int requiredSize = elementIndex + droppedGuids.Count;
                if (arrayProperty.arraySize < requiredSize)
                {
                    arrayProperty.arraySize = requiredSize;
                }

                arrayProperty = serializedObject.FindProperty(arrayPath);
                if (arrayProperty == null)
                {
                    return;
                }

                for (int i = 0; i < droppedGuids.Count; i++)
                {
                    int targetIndex = elementIndex + i;
                    if (targetIndex < 0 || targetIndex >= arrayProperty.arraySize)
                    {
                        break;
                    }

                    SerializedProperty arrayElement = arrayProperty.GetArrayElementAtIndex(targetIndex);
                    SerializedProperty elementValue = arrayElement.FindPropertyRelative(ValueFieldName);
                    if (elementValue != null)
                    {
                        WriteGuid(elementValue, droppedGuids[i]);
                    }
                }
            }
            else
            {
                WriteGuid(valueProp, droppedGuids[0]);
            }

            serializedObject.ApplyModifiedProperties();
            IdentifiableRegistry.MarkDirty();
        }

        public static void WriteGuid(SerializedProperty valueProp, SerializableGuid guid)
        {
            SerializedProperty p1 = valueProp.FindPropertyRelative(nameof(SerializableGuid.Part1));
            SerializedProperty p2 = valueProp.FindPropertyRelative(nameof(SerializableGuid.Part2));
            SerializedProperty p3 = valueProp.FindPropertyRelative(nameof(SerializableGuid.Part3));
            SerializedProperty p4 = valueProp.FindPropertyRelative(nameof(SerializableGuid.Part4));

            p1.longValue = guid.Part1;
            p2.longValue = guid.Part2;
            p3.longValue = guid.Part3;
            p4.longValue = guid.Part4;
        }

        static List<SerializableGuid> ResolveDraggedGuids(IEnumerable<UnityEngine.Object> draggedObjects)
        {
            List<SerializableGuid> guids = new List<SerializableGuid>();

            foreach (UnityEngine.Object draggedObject in draggedObjects)
            {
                if (TryResolveGuid(draggedObject, out SerializableGuid guid))
                {
                    guids.Add(guid);
                }
            }

            return guids;
        }

        static bool TryResolveGuid(UnityEngine.Object source, out SerializableGuid guid)
        {
            guid = SerializableGuid.Empty;

            if (source == null)
            {
                return false;
            }

            if (source is IIdentifiableObject identifiableObject)
            {
                guid = identifiableObject.ID;
                return guid != SerializableGuid.Empty;
            }

            if (source is GameObject gameObject)
            {
                return TryResolveGuidFromComponents(gameObject.GetComponents<MonoBehaviour>(), out guid);
            }

            if (source is Component component)
            {
                return TryResolveGuidFromComponents(component.GetComponents<MonoBehaviour>(), out guid);
            }

            return false;
        }

        static bool TryResolveGuidFromComponents(IEnumerable<MonoBehaviour> components, out SerializableGuid guid)
        {
            guid = SerializableGuid.Empty;

            if (components == null)
            {
                return false;
            }

            IIdentifiableObject identifiableObject = components.OfType<IIdentifiableObject>().FirstOrDefault(obj => obj != null && obj.ID != SerializableGuid.Empty);
            if (identifiableObject == null)
            {
                return false;
            }

            guid = identifiableObject.ID;
            return guid != SerializableGuid.Empty;
        }

        static bool TryGetArrayElementContext(SerializedProperty property, out SerializedProperty arrayProperty, out int elementIndex)
        {
            arrayProperty = null;
            elementIndex = -1;

            const string marker = ".Array.data[";
            string propertyPath = property.propertyPath;
            int markerIndex = propertyPath.LastIndexOf(marker, StringComparison.Ordinal);
            if (markerIndex < 0)
            {
                return false;
            }

            int startIndex = markerIndex + marker.Length;
            int endIndex = propertyPath.IndexOf(']', startIndex);
            if (endIndex < 0)
            {
                return false;
            }

            string indexText = propertyPath.Substring(startIndex, endIndex - startIndex);
            if (!int.TryParse(indexText, out elementIndex))
            {
                return false;
            }

            string arrayPath = propertyPath.Substring(0, markerIndex);
            arrayProperty = property.serializedObject.FindProperty(arrayPath);
            return arrayProperty != null && arrayProperty.isArray;
        }
    }

    /// <summary>
    /// Modern searchable dropdown implementation for selecting Identifiable objects.
    /// </summary>
    public class IdentifiableDropdown : AdvancedDropdown
    {
        [NonSerialized]
        SerializedProperty _valueProp;

        public IdentifiableDropdown(AdvancedDropdownState state, SerializedProperty property) : base(state)
        {
            _valueProp = property;
            minimumSize = new Vector2(250, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Identifiable Objects");

            AdvancedDropdownItem noneItem = new AdvancedDropdownItem("None")
            {
                id = 0
            };
            root.AddChild(noneItem);

            Dictionary<SerializableGuid, List<UnityEngine.Object>> allRegistered = IdentifiableRegistry.GetAllRegistered();
            foreach (var kvp in allRegistered)
            {
                SerializableGuid guid = kvp.Key;
                UnityEngine.Object obj = kvp.Value.FirstOrDefault(o => o != null);
                if (obj == null)
                {
                    continue;
                }

                string groupName = "Assets";
                if (obj is Component comp && comp.gameObject.scene.IsValid())
                {
                    groupName = comp.gameObject.scene.name;
                }
                else if (obj is GameObject go && go.scene.IsValid())
                {
                    groupName = go.scene.name;
                }

                AdvancedDropdownItem sceneGroup = GetOrAddGroup(root, groupName);
                sceneGroup.AddChild(new IdentifiableDropdownItem(obj.name, guid));
            }

            return root;
        }

        AdvancedDropdownItem GetOrAddGroup(AdvancedDropdownItem root, string groupName)
        {
            if (root.children != null)
            {
                foreach (AdvancedDropdownItem child in root.children)
                {
                    if (child.name == groupName)
                    {
                        return child;
                    }
                }
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
            if (_valueProp == null)
            {
                return;
            }

            _valueProp.serializedObject.Update();
            SerializableGuidReferenceDrawer.WriteGuid(_valueProp, guid);
            _valueProp.serializedObject.ApplyModifiedProperties();
            IdentifiableRegistry.MarkDirty();
        }
    }

    /// <summary>
    /// Holds the GUID data within the dropdown structure.
    /// </summary>
    public class IdentifiableDropdownItem : AdvancedDropdownItem
    {
        [NonSerialized]
        SerializableGuid _guid;

        public SerializableGuid Guid => _guid;

        public IdentifiableDropdownItem(string name, SerializableGuid guid) : base(name)
        {
            _guid = guid;
        }
    }
}
#endif