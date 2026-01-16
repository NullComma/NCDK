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

            _blockingEventsManager.MenuRetainable.StateEvent += OnMenuStateChanged;

#if ENABLE_INPUT_SYSTEM
            InputSystem.onEvent += OnInputReceived;
#endif
            Application.quitting += OnAppQuitting;
            Application.focusChanged += OnApplicationFocusChanged;
            
            // Debug to check initial state.
            // If this is False, it means the Manager was initialized BEFORE the Menu View called Retain.
            bool initialState = _blockingEventsManager.MenuRetainable.IsRetained;
            Debug.Log($"[CursorManager] Initialized. Menu Open? {initialState}. Applying initial state...");
            
            RefreshCursorState();
        }

#if ENABLE_INPUT_SYSTEM
        void OnInputReceived(InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;

            bool isMouseOrKeyboard = device is Mouse || device is Keyboard;
            bool isGamepad = device is Gamepad;

            // Filter out events from relevant devices only. 
            // If it's not a mouse/keyboard AND not a gamepad (e.g. some generic HID), we ignore it to avoid flipping state erroneously.
            if (!isMouseOrKeyboard && !isGamepad) return;

            bool currentIsMouseAndKeyboard = isMouseOrKeyboard;

            // 1. Check for device change (Gamepad <-> Mouse)
            bool deviceChanged = currentIsMouseAndKeyboard != _lastInputIsMouseAndKeyboard;
            
            // 2. CRITICAL FAILSAFE:
            // If the user is using Mouse, and they should be seeing the cursor (Menu Open),
            // but the cursor is invisible in the engine, we force a refresh.
            // This fixes the initialization bug where the cursor starts locked unintentionally.
            bool cursorStateDesync = currentIsMouseAndKeyboard && 
                                     _blockingEventsManager.MenuRetainable.IsRetained && 
                                     Cursor.visible == false;

            if (deviceChanged || cursorStateDesync)
            {
                if (deviceChanged) Debug.Log($"[CursorManager] Input device changed: {device.displayName}");
                if (cursorStateDesync) Debug.Log("[CursorManager] Cursor desync detected (Should be visible). Forcing refresh.");

                _lastInputIsMouseAndKeyboard = currentIsMouseAndKeyboard;
                RefreshCursorState();
            }
        }
#endif

        void OnMenuStateChanged(bool onMenu)
        {
            Debug.Log($"[CursorManager] Menu State Changed: {onMenu}");
            RefreshCursorState();
        }

        void OnApplicationFocusChanged(bool hasFocus)
        {
            if (hasFocus) RefreshCursorState();
        }

        void RefreshCursorState()
        {
            // The rule: Show if (Menu Retained) AND (Using Mouse)
            bool menuOpen = _blockingEventsManager.MenuRetainable.IsRetained;
            bool isMouse = _lastInputIsMouseAndKeyboard;
            
            bool shouldShow = menuOpen && isMouse;
            
            SetCursorState(shouldShow);
        }

        static void SetCursorState(bool visible)
        {
            Debug.Log($"[CursorManager] SetCursorState: {visible} | Frame: {Time.frameCount} | Cursor.visible was: {Cursor.visible}");
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }
        
        public void Dispose()
        {
            if (_blockingEventsManager != null)
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
            SetCursorState(true);
#endif
        }
    }
}