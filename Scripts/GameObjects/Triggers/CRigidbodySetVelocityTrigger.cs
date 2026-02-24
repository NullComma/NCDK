using System;
using UnityEngine;

namespace EnigmaCore {
	[RequireComponent(typeof(Rigidbody))]
	public class CRigidbodySetVelocityTrigger : CAutoTriggerCompBase {
		[SerializeField] Vector3 _setVelocityAmount = Vector3.forward;
		[NonSerialized] Rigidbody _rb;
		
		
		
		
		protected override void Awake() {
			_rb = GetComponent<Rigidbody>();
			base.Awake();
		}

		protected override void TriggerEvent() {
			// Unity 6+ uses linearVelocity, older versions use velocity
			#if UNITY_6000_0_OR_NEWER
			_rb.linearVelocity = transform.TransformDirection(_setVelocityAmount);
			#else
			_rb.velocity = transform.TransformDirection(_setVelocityAmount);
			#endif
		}
		
	}
}
