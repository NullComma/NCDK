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
        [NonSerialized] readonly BlockingEventsManager _blockingEventsManager;
        [NonSerialized] bool _lastInputIsMouseAndKeyboard = true;

        public CursorManager(BlockingEventsManager blockingEventsManager)
        {
            _blockingEventsManager = blockingEventsManager;

            _blockingEventsManager.MenuRetainable.StateEvent += OnMenuStateChanged;

            _lastInputIsMouseAndKeyboard = EInput.CurrentDevice == InputDeviceType.MouseAndKeyboard;
            EInput.OnDeviceChanged += OnDeviceChanged;

#if ENABLE_INPUT_SYSTEM
            InputSystem.onEvent += FailsafeInputCheck;
#endif
            Application.quitting += OnAppQuitting;
            Application.focusChanged += OnApplicationFocusChanged;
        }

        void OnDeviceChanged(InputDeviceType device)
        {
            bool isMouseAndKeyboard = device == InputDeviceType.MouseAndKeyboard;
            if (_lastInputIsMouseAndKeyboard != isMouseAndKeyboard)
            {
                _lastInputIsMouseAndKeyboard = isMouseAndKeyboard;
                Debug.Log($"[CursorManager] Input device changed: {device}");
                RefreshCursorState();
            }
        }

#if ENABLE_INPUT_SYSTEM
        void FailsafeInputCheck(InputEventPtr eventPtr, InputDevice device)
        {
            if (!_lastInputIsMouseAndKeyboard || !_blockingEventsManager.MenuRetainable.IsRetained) return;
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;
            
            if (device is Mouse || device is Keyboard)
            {
                if (Cursor.visible == false)
                {
                    Debug.Log("[CursorManager] Cursor desync detected (Should be visible). Forcing refresh.");
                    RefreshCursorState();
                }
            }
        }
#endif

        void OnMenuStateChanged(bool onMenu)
        {
            RefreshCursorState();
        }

        void OnApplicationFocusChanged(bool hasFocus)
        {
            if (hasFocus) RefreshCursorState();
        }

        void RefreshCursorState()
        {
            bool menuOpen = _blockingEventsManager.MenuRetainable.IsRetained;
            bool isMouse = _lastInputIsMouseAndKeyboard;
            
            bool shouldShow = menuOpen && isMouse;
            
            SetCursorState(shouldShow);
        }

        static void SetCursorState(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }
        
        public void Dispose()
        {
            if (_blockingEventsManager != null)
                _blockingEventsManager.MenuRetainable.StateEvent -= OnMenuStateChanged;
            
            EInput.OnDeviceChanged -= OnDeviceChanged;

#if ENABLE_INPUT_SYSTEM
            InputSystem.onEvent -= FailsafeInputCheck;
#endif
            Application.quitting -= OnAppQuitting;
            Application.focusChanged -= OnApplicationFocusChanged;
        }

        void OnAppQuitting()
        {
            Dispose();
#if UNITY_EDITOR
            SetCursorState(true);
#endif
        }
    }
}