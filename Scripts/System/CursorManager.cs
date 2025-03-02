using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EnigmaCore {
	public class CursorManager
	{
		[NonSerialized] readonly CBlockingEventsManager _blockingEventsManager;

		public CursorManager(CBlockingEventsManager blockingEventsManager) {
	        _blockingEventsManager = blockingEventsManager;

	        _blockingEventsManager.MenuRetainable.StateEvent += (onMenu) => {
                if (!onMenu) {
                    SetCursorState(false);
                    return;
                }
                ShowMouseIfNeeded();
            };

            CApplication.QuittingEvent += OnAppQuitting;
        }
		
		void OnAppQuitting()
		{
			CApplication.QuittingEvent -= OnAppQuitting;
			#if UNITY_EDITOR
			SetCursorState(true);
			#endif
		}

		static void SetCursorState(bool visible)
		{
			Cursor.visible = visible;
			Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
		}

        public void ShowMouseIfNeeded() {
            if (Gamepad.current != null && Gamepad.current.enabled) return;
            SetCursorState(true);
        }

		public void HideCursor() {
            SetCursorState(false);
        }

	}
}