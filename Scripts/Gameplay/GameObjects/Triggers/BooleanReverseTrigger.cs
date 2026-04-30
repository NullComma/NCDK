using UnityEngine;

namespace NCDK {
	public class BooleanReverseTrigger : MonoBehaviour {

		[SerializeField] CUnityEventBool _reversedBoolEvent;
		
		
		public void TriggerReversed(bool value) {
			_reversedBoolEvent?.Invoke(!value);
		}
		
	}
}
