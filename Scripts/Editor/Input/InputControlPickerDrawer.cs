#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EnigmaCore.Editor.Input
{
    [CustomPropertyDrawer(typeof(EnigmaCore.Input.InputControlPickerAttribute))]
    public class InputControlPickerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [InputControlPicker] on string fields.");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Draw the label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // The button that opens the picker
            string currentPath = property.stringValue;
            string displayString = string.IsNullOrEmpty(currentPath) ? "<Select Control>" : currentPath;

            // In modern Unity Input System, InputControlPickerDropdown is internal/inaccessible.
            // We use a custom AdvancedDropdown instead.
            if (GUI.Button(position, displayString, EditorStyles.popup))
            {
                var dropdown = new InputControlAdvancedDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), path =>
                {
                    property.stringValue = path;
                    property.serializedObject.ApplyModifiedProperties();
                });
                
                dropdown.Show(position);
            }

            EditorGUI.EndProperty();
        }
    }

    public class InputControlAdvancedDropdown : UnityEditor.IMGUI.Controls.AdvancedDropdown
    {
        private Action<string> _onSelected;

        public InputControlAdvancedDropdown(UnityEditor.IMGUI.Controls.AdvancedDropdownState state, Action<string> onSelected) : base(state)
        {
            _onSelected = onSelected;
            this.minimumSize = new Vector2(250, 300);
        }

        protected override UnityEditor.IMGUI.Controls.AdvancedDropdownItem BuildRoot()
        {
            var root = new UnityEditor.IMGUI.Controls.AdvancedDropdownItem("Input Controls");
            
            // Generate common paths manually since the internal Unity picker is hidden
            var gamepad = new UnityEditor.IMGUI.Controls.AdvancedDropdownItem("Gamepad");
            gamepad.AddChild(new InputControlDropdownItem("Button South", "<Gamepad>/buttonSouth"));
            gamepad.AddChild(new InputControlDropdownItem("Button East", "<Gamepad>/buttonEast"));
            gamepad.AddChild(new InputControlDropdownItem("Button West", "<Gamepad>/buttonWest"));
            gamepad.AddChild(new InputControlDropdownItem("Button North", "<Gamepad>/buttonNorth"));
            gamepad.AddChild(new InputControlDropdownItem("D-Pad Up", "<Gamepad>/dpad/up"));
            gamepad.AddChild(new InputControlDropdownItem("D-Pad Down", "<Gamepad>/dpad/down"));
            gamepad.AddChild(new InputControlDropdownItem("D-Pad Left", "<Gamepad>/dpad/left"));
            gamepad.AddChild(new InputControlDropdownItem("D-Pad Right", "<Gamepad>/dpad/right"));
            gamepad.AddChild(new InputControlDropdownItem("Left Shoulder", "<Gamepad>/leftShoulder"));
            gamepad.AddChild(new InputControlDropdownItem("Right Shoulder", "<Gamepad>/rightShoulder"));
            gamepad.AddChild(new InputControlDropdownItem("Left Trigger", "<Gamepad>/leftTrigger"));
            gamepad.AddChild(new InputControlDropdownItem("Right Trigger", "<Gamepad>/rightTrigger"));
            gamepad.AddChild(new InputControlDropdownItem("Start", "<Gamepad>/start"));
            gamepad.AddChild(new InputControlDropdownItem("Select", "<Gamepad>/select"));

            var mouse = new UnityEditor.IMGUI.Controls.AdvancedDropdownItem("Mouse");
            mouse.AddChild(new InputControlDropdownItem("Left Button", "<Mouse>/leftButton"));
            mouse.AddChild(new InputControlDropdownItem("Right Button", "<Mouse>/rightButton"));
            mouse.AddChild(new InputControlDropdownItem("Middle Button", "<Mouse>/middleButton"));

            var keyboard = new UnityEditor.IMGUI.Controls.AdvancedDropdownItem("Keyboard");
            keyboard.AddChild(new InputControlDropdownItem("Space", "<Keyboard>/space"));
            keyboard.AddChild(new InputControlDropdownItem("Enter", "<Keyboard>/enter"));
            keyboard.AddChild(new InputControlDropdownItem("Escape", "<Keyboard>/escape"));
            keyboard.AddChild(new InputControlDropdownItem("E", "<Keyboard>/e"));
            keyboard.AddChild(new InputControlDropdownItem("F", "<Keyboard>/f"));

            root.AddChild(gamepad);
            root.AddChild(mouse);
            root.AddChild(keyboard);

            return root;
        }

        protected override void ItemSelected(UnityEditor.IMGUI.Controls.AdvancedDropdownItem item)
        {
            if (item is InputControlDropdownItem controlItem)
            {
                _onSelected?.Invoke(controlItem.ControlPath);
            }
        }
    }

    public class InputControlDropdownItem : UnityEditor.IMGUI.Controls.AdvancedDropdownItem
    {
        public string ControlPath { get; private set; }

        public InputControlDropdownItem(string name, string controlPath) : base(name)
        {
            ControlPath = controlPath;
        }
    }
}
#endif
