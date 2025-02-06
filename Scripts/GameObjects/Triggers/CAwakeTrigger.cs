using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore {
	public class CAwakeTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void Awake() {
			TriggerEvent?.Invoke();
		}
		
	}
}