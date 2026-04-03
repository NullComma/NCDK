using UnityEngine;

namespace NullCore {
	public abstract class MonoBehaviourUpdateExecutionLoopTime : MonoBehaviour {

		
		[SerializeField] private MonoBehaviourExecutionLoop _executionTime;
		

		#region <<---------- MonoBehaviour ---------->>

		private void Update() {
			if (this._executionTime != MonoBehaviourExecutionLoop.Update) return;
			this.Execute(Time.deltaTime);
		}

		private void FixedUpdate() {
			if (this._executionTime != MonoBehaviourExecutionLoop.FixedUpdate) return;
			this.Execute(Time.deltaTime);
		}

		private void LateUpdate() {
			if (this._executionTime != MonoBehaviourExecutionLoop.LateUpdate) return;
			this.Execute(Time.deltaTime);
		}

		#endregion <<---------- MonoBehaviour ---------->>

		
		protected abstract void Execute(float deltaTime);
		

	}
}
