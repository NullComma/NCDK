using UnityEngine;
using UnityEngine.Events;

namespace NCDK {
	public class DisableTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void OnDisable() {
			TriggerEvent?.Invoke();
		}
		
	}
}