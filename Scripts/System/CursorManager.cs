using System;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
#endif

namespace EnigmaCore
{
    public class CursorManager : IDisposable
    {
        [NonSerialized] readonly CBlockingEventsManager _blockingEventsManager;
        [NonSerialized] bool _lastInputIsMouseAndKeyboard = true;

        public CursorManager(CBlockingEventsManager blockingEventsManager)
        {
            _blockingEventsManager = blockingEventsManager;

            // Subscribe to menu events to control cursor state
            _blockingEventsManager.MenuRetainable.StateEvent += OnMenuStateChanged;

            // Subscribe to global input events to detect device usage
#if ENABLE_INPUT_SYSTEM
            InputSystem.onEvent += OnInputReceived;
#endif
            Application.quitting += OnAppQuitting;
            
            // Set initial state based on menu
            OnMenuStateChanged(_blockingEventsManager.MenuRetainable.IsRetained);
        }

        /// <summary>
        /// This is the core of the new logic. It listens to all input events.
        /// </summary>
        void OnInputReceived(InputEventPtr eventPtr, InputDevice device)
        {
            // We only care about events that represent actual user interaction.
            // This is a simple way to filter out noise from devices updating their state without user input.
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            {
                return;
            }

            bool currentIsMouseAndKeyboard = device is Mouse || device is Keyboard;

            // If the device type has changed, update the state
            if (currentIsMouseAndKeyboard != _lastInputIsMouseAndKeyboard)
            {
                Debug.Log($"Input device changed to {(currentIsMouseAndKeyboard ? "Mouse/Keyboard" : "Gamepad/Other")}");
                _lastInputIsMouseAndKeyboard = currentIsMouseAndKeyboard;
                SetCursorState(_blockingEventsManager.MenuRetainable.IsRetained && _lastInputIsMouseAndKeyboard);
            }
        }

        void OnMenuStateChanged(bool onMenu)
        {
            SetCursorState(onMenu && _lastInputIsMouseAndKeyboard);
        }

        static void SetCursorState(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        public void Dispose()
        {
            // Unsubscribe from events to prevent memory leaks
            _blockingEventsManager.MenuRetainable.StateEvent -= OnMenuStateChanged;
#if ENABLE_INPUT_SYSTEM
            InputSystem.onEvent -= OnInputReceived;
#endif
            Application.quitting -= OnAppQuitting;
        }

        void OnAppQuitting()
        {
            Dispose();
#if UNITY_EDITOR
            // Ensure cursor is visible when exiting play mode in the editor
            SetCursorState(true);
#endif
        }
    }
}