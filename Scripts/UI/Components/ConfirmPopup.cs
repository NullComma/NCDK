using System;
using TMPro;

using UnityEngine;

namespace EnigmaCore.UI {
	public class ConfirmPopup : View {
		
		[Header("Confirmation")]
		[SerializeField] CUIButton _buttonConfirm;
        [SerializeField] TextMeshProUGUI _title;
        

		
		public void SetupPopup(Action onConfirm, string title) {
			if(onConfirm == null)
			{
				Debug.LogError($"Confirm Popup '{gameObject.name}' has no confirm action assigned, closing popup.",this);
				this.Close();
			}
			// confirm exit
			_buttonConfirm.Button.interactable = true;
			_buttonConfirm.ClickEvent += () => {
				Debug.Log($"SUBMIT: Confirm Popup '{gameObject.name}'",this);
				this.Close();
				onConfirm?.Invoke();
			};
					
			_eventSystem.SetSelectedGameObject(_buttonConfirm.gameObject);
            
            _title.text = title;
		}

	}
}
