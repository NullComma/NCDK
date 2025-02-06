using UnityEngine;

namespace EnigmaCore {
	public class CResetTransformTrigger : MonoBehaviour {

		public void ResetRotation(Transform target) {
			target.localRotation = Quaternion.identity;
		}

		public void ResetPosition(Transform target) {
			target.localPosition = Vector3.zero;
		}
		
	}
}