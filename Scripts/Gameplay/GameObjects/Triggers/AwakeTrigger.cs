using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore {
	public class AwakeTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void Awake() {
			TriggerEvent?.Invoke();
		}
		
	}
}