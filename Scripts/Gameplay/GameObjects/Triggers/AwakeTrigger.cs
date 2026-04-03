using UnityEngine;
using UnityEngine.Events;

namespace NullCore {
	public class AwakeTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void Awake() {
			TriggerEvent?.Invoke();
		}
		
	}
}