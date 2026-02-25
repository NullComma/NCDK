using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore {
	public class DisableTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void OnDisable() {
			TriggerEvent?.Invoke();
		}
		
	}
}