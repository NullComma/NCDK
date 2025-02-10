using UnityEngine;

namespace EnigmaCore {
	public abstract class CAutoTriggerCompBase : MonoBehaviour {

		[SerializeField] MonoBehaviourExecutionTime executionTime = MonoBehaviourExecutionTime.Awake;

		protected virtual void Awake() {
			if (executionTime != MonoBehaviourExecutionTime.Awake) return;
			TriggerEvent();
		}

		protected virtual  void Start() {
			if (!enabled) return;
			if (executionTime != MonoBehaviourExecutionTime.Start) return;
			TriggerEvent();
		}

		protected virtual  void OnEnable() {
			if (!enabled) return;
			if (executionTime != MonoBehaviourExecutionTime.OnEnable) return;
			TriggerEvent();
		}

		protected abstract void TriggerEvent();

	}
}