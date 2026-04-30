using UnityEngine;
using UnityEngine.Events;

namespace NCDK {
	public class AwakeTrigger : MonoBehaviour {
		[SerializeField] UnityEvent TriggerEvent;

		void Awake() {
			TriggerEvent?.Invoke();
		}
		
	}
}