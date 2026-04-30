using UnityEngine;

namespace NCDK {
	public class OnEditorTrigger : AutoTriggerCompBase {

		[SerializeField] CUnityEventBool _isOnEditorEvent;
		[SerializeField] CUnityEventBool _isNotOnEditorEvent;
		
		
		
		
		protected override void TriggerEvent() {
			var isEditor = Application.isEditor;
			_isOnEditorEvent?.Invoke(isEditor);
			_isNotOnEditorEvent?.Invoke(!isEditor);
		}
	}
}
