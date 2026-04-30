using UnityEngine;

namespace NCDK {
	public class AwakeSetActiveState : MonoBehaviour {

		[SerializeField] bool _activeStateOnAwake;

		void Awake() {
			gameObject.SetActive(_activeStateOnAwake);
			this.CDestroy();
		}
		
	}
}