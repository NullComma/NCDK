using UnityEngine;

namespace EnigmaCore {
	public class DestroyGameObjectTrigger : AutoTriggerCompBase {

		[SerializeField] bool _destroyThis;

		
		
		
		public void DestroySelfGameObject() {
			DestroyGameObject(gameObject);
		}

		public void DestroyGameObject(GameObject goToDestroy) {
			if (goToDestroy == null) {
				Debug.Log($"{nameof(DestroyGameObjectTrigger)} received a request to destroy an empty game object.", gameObject);
				return;
			}
			Debug.Log($"{nameof(DestroyGameObjectTrigger)} <color=orange>destroying game object</color> '{goToDestroy.name}'.", goToDestroy);
			goToDestroy.CDestroy();
		}

		protected override void TriggerEvent() {
			if (!_destroyThis) return;
			Debug.Log($"[{nameof(DestroyGameObjectTrigger)}] '{name}' <color=orange>self destroying</color>.");
			gameObject.CDestroy();
		}
	}
}
