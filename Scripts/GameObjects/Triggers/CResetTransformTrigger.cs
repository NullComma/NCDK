using UnityEngine;

namespace EnigmaCore {
	public class CResetTransformTrigger : MonoBehaviour
	{
		Vector3 DefaultPosition => Vector3.zero;
		Quaternion DefaultRotation => Quaternion.identity;
		
		public void ResetThisPosition()
		{
			ResetPosition(this.transform);
		}

		public void ResetThisRotation()
		{
			ResetRotation(this.transform);
		}
		
		public void ResetThisPositionAndRotation()
		{
			ResetPositionAndRotation(this.transform);
		}

		public void ResetPosition(Transform target) {
			target.localPosition = DefaultPosition;
		}
		
		public void ResetRotation(Transform target) {
			target.localRotation = Quaternion.identity;
		}

		public void ResetPositionAndRotation(Transform target)
		{
			target.SetLocalPositionAndRotation(DefaultPosition, DefaultRotation);
		}
	}
}