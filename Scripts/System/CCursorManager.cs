using System;
using EnigmaCore.DependecyInjection;
using UnityEngine;

namespace EnigmaCore {
	public class CCursorManager
	{
		[NonSerialized] readonly CBlockingEventsManager _blockingEventsManager;
		[NonSerialized] readonly CInputManager _inputManager;

        public CCursorManager(CBlockingEventsManager blockingEventsManager, CInputManager inputManager) {
	        _blockingEventsManager = blockingEventsManager;
			_inputManager = inputManager;

	        _blockingEventsManager.MenuRetainable.StateEvent += (onMenu) => {
                if (!onMenu) {
                    SetCursorState(false);
                    return;
                }
                ShowMouseIfNeeded();
            };

	        _inputManager.InputTypeChanged += OnInputTypeChanged;
        }

        #if UNITY_EDITOR
		~CCursorManager() {
			SetCursorState(true);
		}
		#endif


		void OnInputTypeChanged(object sender, CInputManager.InputType inputType) {
			SetCursorState(_inputManager.ActiveInputType.IsMouseOrKeyboard() && DIContainer.Resolve<CBlockingEventsManager>().IsInMenu);
		}

		static void SetCursorState(bool visible)
		{
			Cursor.visible = visible;
			Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
		}

        public void ShowMouseIfNeeded() {
            if (!_inputManager.ActiveInputType.IsMouseOrKeyboard()) return;
            SetCursorState(true);
        }

		public void HideCursor() {
            SetCursorState(false);
        }

	}
}