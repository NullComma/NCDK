#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace EnigmaCore
{
    public static class EInput
    {
        /// <summary>
        /// Checks across all relevant devices if a skip button is currently being held down.
        /// </summary>
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
            return Input.anyKey;
#endif
        }
        
    }
}