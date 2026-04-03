using UnityEngine;
using UnityEngine.Serialization;

namespace NullCore.Data {
	[System.Serializable]
	public class AttackData {

		[FormerlySerializedAs("ScriptableObject")] public HitInfoData data;
		public Transform AttackerTransform;
		public Vector3 HitPointPosition;
		public float Damage => data.RawDamage;

		public AttackData(HitInfoData data, Vector3 hitPointPosition, Transform attackerTransform) {
			if (data == null) {
				// log error:
				Debug.LogError("AttackData: data is null");
			}
			this.data = data;
			AttackerTransform = attackerTransform;
			HitPointPosition = hitPointPosition;
		}
		
	}
}