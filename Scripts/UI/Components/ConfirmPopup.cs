using System;
using TMPro;

using UnityEngine;

namespace EnigmaCore.UI {
	public class ConfirmPopup : View {
		
		[Header("Confirmation")]
		[SerializeField] CUIButton _buttonConfirm;
        [SerializeField] TextMeshProUGUI _title;
        

		
		public void SetupPopup(EventHandler onConfirm, string title) {
			// confirm exit
			_buttonConfirm.Button.interactable = true;
			_buttonConfirm.ClickEvent += () => {
				Debug.Log($"SUBMIT: Confirm Popup '{gameObject.name}'",this);
				this.Close();
				onConfirm?.Invoke(this,EventArgs.Empty);
			};
					
			_eventSystem.SetSelectedGameObject(_buttonConfirm.gameObject);
            
            _title.text = title;
		}

	}
}
