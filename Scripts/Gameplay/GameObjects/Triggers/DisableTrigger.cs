using UnityEngine;
using UnityEngine.Events;

namespace NullCore {
	public class DisableTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void OnDisable() {
			TriggerEvent?.Invoke();
		}
		
	}
}