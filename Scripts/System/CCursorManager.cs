using EnigmaCore.DependecyInjection;
using UnityEngine;

namespace EnigmaCore {
	public class CCursorManager {

        #region <<---------- Initializers ---------->>

        public CCursorManager() {
            DIContainer.Resolve<CBlockingEventsManager>().MenuRetainable.StateEvent += (onMenu) => {
                if (!onMenu) {
                    SetCursorState(false);
                    return;
                }
                ShowMouseIfNeeded();
            };

            DIContainer.Resolve<CInputManager>().InputTypeChanged += OnInputTypeChanged;
        }

        #if UNITY_EDITOR
		~CCursorManager() {
			SetCursorState(true);
		}
		#endif

		#endregion <<---------- Initializers ---------->>


		void OnInputTypeChanged(object sender, CInputManager.InputType inputType) {
			SetCursorState(DIContainer.Resolve<CInputManager>().ActiveInputType.IsMouseOrKeyboard() && DIContainer.Resolve<CBlockingEventsManager>().IsInMenu);
		}

		static void SetCursorState(bool visible)
		{
			Cursor.visible = visible;
			Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
		}

        public void ShowMouseIfNeeded() {
            if (!DIContainer.Resolve<CInputManager>().ActiveInputType.IsMouseOrKeyboard()) return;
            SetCursorState(true);
        }

		public void HideCursor() {
            SetCursorState(false);
        }

	}
}