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

            // Subscribe to menu events 
            _blockingEventsManager.MenuRetainable.StateEvent += OnMenuStateChanged;

            // Subscribe to global input events
#if ENABLE_INPUT_SYSTEM
            InputSystem.onEvent += OnInputReceived;
#endif
            Application.quitting += OnAppQuitting;
            Application.focusChanged += OnApplicationFocusChanged;
            
            RefreshCursorState();
        }

        void OnInputReceived(InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            {
                return;
            }

            bool currentIsMouseAndKeyboard = device is Mouse || device is Keyboard;

            if (currentIsMouseAndKeyboard != _lastInputIsMouseAndKeyboard)
            {
                Debug.Log($"Input device changed to {(currentIsMouseAndKeyboard ? "Mouse/Keyboard" : "Gamepad/Other")}");
                _lastInputIsMouseAndKeyboard = currentIsMouseAndKeyboard;
                RefreshCursorState();
            }
        }

        void OnMenuStateChanged(bool onMenu)
        {
            RefreshCursorState();
        }

        void OnApplicationFocusChanged(bool hasFocus)
        {
            if (hasFocus)
            {
                RefreshCursorState();
            }
        }

        void RefreshCursorState()
        {
            bool shouldShow = _blockingEventsManager.MenuRetainable.IsRetained && _lastInputIsMouseAndKeyboard;
            SetCursorState(shouldShow);
        }

        static void SetCursorState(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        public void Dispose()
        {
            _blockingEventsManager.MenuRetainable.StateEvent -= OnMenuStateChanged;
#if ENABLE_INPUT_SYSTEM
            InputSystem.onEvent -= OnInputReceived;
#endif
            Application.quitting -= OnAppQuitting;
            Application.focusChanged -= OnApplicationFocusChanged;
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