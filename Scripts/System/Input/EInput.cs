using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Switch;
#endif

namespace EnigmaCore
{
    public enum InputDeviceType { MouseAndKeyboard, Gamepad }
    
    public enum GamepadLayoutType
    {
        Auto = 0,
        Xbox = 1,
        PlayStation = 2,
        Nintendo = 3
    }

    public static class EInput
    {
        public static InputDeviceType CurrentDevice { get; private set; } = InputDeviceType.MouseAndKeyboard;
        
        /// <summary>
        /// What the user selected in options (Auto defaults to detecting).
        /// </summary>
        public static GamepadLayoutType PreferredGamepadLayout { get; private set; } = GamepadLayoutType.Auto;
        
        public static event Action<InputDeviceType> OnDeviceChanged;
        public static event Action OnGamepadLayoutChanged;

        private const string PREF_KEY_GAMEPAD_LAYOUT = "EnigmaCore_GamepadLayout";

#if ENABLE_INPUT_SYSTEM
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitTracking()
        {
            CurrentDevice = InputDeviceType.MouseAndKeyboard;
            PreferredGamepadLayout = (GamepadLayoutType)PlayerPrefs.GetInt(PREF_KEY_GAMEPAD_LAYOUT, (int)GamepadLayoutType.Auto);
            
            OnDeviceChanged = null;
            OnGamepadLayoutChanged = null;
            
            InputSystem.onEvent -= OnInputReceived; 
            InputSystem.onEvent += OnInputReceived;
            
            Application.quitting -= OnAppQuitting;
            Application.quitting += OnAppQuitting;
        }

        private static void OnAppQuitting()
        {
            InputSystem.onEvent -= OnInputReceived;
        }

        private static void OnInputReceived(InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;

            bool isMouseOrKeyboard = device is Mouse || device is Keyboard;
            bool isGamepad = device is Gamepad;

            if (!isMouseOrKeyboard && !isGamepad) return;

            InputDeviceType newDevice = isMouseOrKeyboard ? InputDeviceType.MouseAndKeyboard : InputDeviceType.Gamepad;

            if (CurrentDevice != newDevice)
            {
                CurrentDevice = newDevice;
                OnDeviceChanged?.Invoke(CurrentDevice);
            }
        }
#endif

        /// <summary>
        /// Call this from UI Options to enforce a Gamepad style.
        /// </summary>
        public static void SetGamepadLayoutPreference(GamepadLayoutType layout)
        {
            if (PreferredGamepadLayout != layout)
            {
                PreferredGamepadLayout = layout;
                PlayerPrefs.SetInt(PREF_KEY_GAMEPAD_LAYOUT, (int)layout);
                OnGamepadLayoutChanged?.Invoke();
            }
        }

        /// <summary>
        /// Returns the final GamepadLayout to use. 
        /// If set to Auto, it tries to detect the vendor via InputSystem (PlayStation/Switch). 
        /// Otherwise it defaults to Xbox.
        /// </summary>
        public static GamepadLayoutType GetResolvedGamepadLayout()
        {
            if (PreferredGamepadLayout != GamepadLayoutType.Auto)
            {
                return PreferredGamepadLayout; 
            }

#if ENABLE_INPUT_SYSTEM
            Gamepad pad = Gamepad.current;
            if (pad != null)
            {
                if (pad is DualShockGamepad || pad.name.Contains("DualShock", StringComparison.OrdinalIgnoreCase) || pad.name.Contains("DualSense", StringComparison.OrdinalIgnoreCase))
                {
                    return GamepadLayoutType.PlayStation;
                }
                
                if (pad is SwitchProControllerHID || pad.name.Contains("Switch", StringComparison.OrdinalIgnoreCase) || pad.name.Contains("ProController", StringComparison.OrdinalIgnoreCase))
                {
                    return GamepadLayoutType.Nintendo;
                }
            }
#endif
            // Safe fallback
            return GamepadLayoutType.Xbox;
        }

        public static bool IsAnySkipInputHeld()
        {
#if ENABLE_INPUT_SYSTEM
            // Check Keyboard: anyKey is for press, but a simple check for common keys is more reliable for held state.
            bool keyboardHeld = Keyboard.current != null && (Keyboard.current.spaceKey.isPressed || Keyboard.current.enterKey.isPressed || Keyboard.current.escapeKey.isPressed);
            
            // Check Gamepad
            bool gamepadHeld = false;
            if (Gamepad.current != null)
            {
                gamepadHeld = Gamepad.current.buttonSouth.isPressed || Gamepad.current.buttonEast.isPressed || Gamepad.current.startButton.isPressed;
            }

            // Check Mouse
            bool mouseHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
            
            return keyboardHeld || gamepadHeld || mouseHeld;
#else
            // Fallback for old Input Manager
            return UnityEngine.Input.anyKey;
#endif
        }
        
    }
}