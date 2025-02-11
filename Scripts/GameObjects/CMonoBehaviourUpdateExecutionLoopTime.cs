using UnityEngine;

namespace EnigmaCore {
	public abstract class MonoBehaviourUpdateExecutionLoopTime : MonoBehaviour {

		
		[SerializeField] private MonoBehaviourExecutionLoop _executionTime;
		

		#region <<---------- MonoBehaviour ---------->>

		private void Update() {
			if (this._executionTime != MonoBehaviourExecutionLoop.Update) return;
			this.Execute(ETime.DeltaTimeScaled);
		}

		private void FixedUpdate() {
			if (this._executionTime != MonoBehaviourExecutionLoop.FixedUpdate) return;
			this.Execute(ETime.DeltaTimeScaled);
		}

		private void LateUpdate() {
			if (this._executionTime != MonoBehaviourExecutionLoop.LateUpdate) return;
			this.Execute(ETime.DeltaTimeScaled);
		}

		#endregion <<---------- MonoBehaviour ---------->>

		
		protected abstract void Execute(float deltaTime);
		

	}
}
