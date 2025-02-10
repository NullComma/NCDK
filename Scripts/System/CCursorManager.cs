using UnityEngine;

namespace EnigmaCore {
	public class CCursorManager {

        #region <<---------- Initializers ---------->>

        public CCursorManager() {
            Static.BlockingEventsManager.MenuRetainable.StateEvent += (onMenu) => {
                if (!onMenu) {
                    SetCursorState(false);
                    return;
                }
                ShowMouseIfNeeded();
            };

            Static.InputManager.InputTypeChanged += OnInputTypeChanged;
        }

        #if UNITY_EDITOR
		~CCursorManager() {
			SetCursorState(true);
		}
		#endif

		#endregion <<---------- Initializers ---------->>


		void OnInputTypeChanged(object sender, CInputManager.InputType inputType) {
			SetCursorState(Static.InputManager.ActiveInputType.IsMouseOrKeyboard() && Static.BlockingEventsManager.IsInMenu);
		}

		static void SetCursorState(bool visible)
		{
			Cursor.visible = visible;
			Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
		}

        public void ShowMouseIfNeeded() {
            if (!Static.InputManager.ActiveInputType.IsMouseOrKeyboard()) return;
            SetCursorState(true);
        }

		public void HideCursor() {
            SetCursorState(false);
        }

	}
}