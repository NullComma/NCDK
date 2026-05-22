using System;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace NCDK.UI {
	public class ConfirmPopup : View {
		
		[Header("Confirmation")]
		[SerializeField] Button _buttonConfirm;
        [SerializeField] TextMeshProUGUI _title;
		[NonSerialized] Action onConfirmAction;

		
		public void SetupPopup(Action onConfirm, string title) {
			if(onConfirm == null)
			{
				Debug.LogError($"Confirm Popup '{gameObject.name}' has no confirm action assigned, closing popup.",this);
				this.Close();
			}
			onConfirmAction = onConfirm;
			
			// confirm exit
			_buttonConfirm.interactable = true;
			_buttonConfirm.onClick.AddListener(ButtonConfirmClick);
					
			_eventSystem.SetSelectedGameObject(_buttonConfirm.gameObject);
            
            _title.text = title;
		}
		void ButtonConfirmClick()
		{
			Debug.Log($"SUBMIT: Confirm Popup '{gameObject.name}'",this);
			this.Close();
			onConfirmAction.Invoke();
		}
	}
}
