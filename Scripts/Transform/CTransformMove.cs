using System;
using UnityEngine;

namespace EnigmaCore {
	public class CTransformMove : MonoBehaviour {
		
		[SerializeField] Vector3 _localDirectionAndSpeed = Vector3.forward;
		[NonSerialized] Transform _transform;
		[NonSerialized] Vector3 _newPosition;


		void Awake() {
			_transform = transform;
		}

		void Update() {
			_newPosition = _transform.position;
			_newPosition += _transform.forward * (_localDirectionAndSpeed.z * Time.deltaTime);
			_newPosition += _transform.right * (_localDirectionAndSpeed.x * Time.deltaTime);
			_newPosition += _transform.up * (_localDirectionAndSpeed.y * Time.deltaTime);
			_transform.position = _newPosition;
		}
		
	}
}
