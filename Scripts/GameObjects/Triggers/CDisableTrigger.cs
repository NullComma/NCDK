using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore {
	public class CDisableTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void OnDisable() {
			TriggerEvent?.Invoke();
		}
		
	}
}