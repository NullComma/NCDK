using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnigmaCore {
    [ExecuteAlways]
    [DefaultExecutionOrder(350)]
	public class TransformFollower : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
        
		[SerializeField] MonoBehaviourExecutionLoop executionLoop = MonoBehaviourExecutionLoop.LateUpdate;
		[SerializeField] Transform _transformToFollow;
        public Transform TransformToFollow => _transformToFollow;
		[Header("Position")]
		[SerializeField] Vector3 _followOffset = Vector3.zero;
		[SerializeField] FollowTypeEnum _followType;
		[SerializeField] float _followSpeed = 10f;
		[NonSerialized] Transform _myTransform;

        [SerializeField] bool _ignoreTimeScale;
		[SerializeField] bool _ignoreXAxis;
		[SerializeField] bool _ignoreYAxis;
		[SerializeField] bool _ignoreZAxis;

        [SerializeField] Vector3 _positionMultiplier = Vector3.one;

		[Header("Rotation")]
		[SerializeField] bool _followRotation;

		public event Action<Transform> TransformToFollowChanged = delegate { };

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- Enums ---------->>

		enum FollowTypeEnum {
			instant, smooth
		}

		#endregion <<---------- Enums ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			_myTransform = transform;
		}

        protected virtual void OnEnable() {
			CheckIfWillMove();
			#if UNITY_EDITOR
			if (_myTransform == null) _myTransform = transform;
			#endif
		}

        protected virtual void Update() {
			#if UNITY_EDITOR
			if (!Application.isPlaying && UnityEditor.Selection.activeGameObject != gameObject) return;
			#endif
			if (executionLoop != MonoBehaviourExecutionLoop.Update && Application.isPlaying) return;
            Execute(_ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
        }

		protected virtual void FixedUpdate() {
			#if UNITY_EDITOR
			if (!Application.isPlaying && UnityEditor.Selection.activeGameObject != gameObject) return;
			#endif
			if (executionLoop != MonoBehaviourExecutionLoop.FixedUpdate && Application.isPlaying) return;
            Execute(_ignoreTimeScale ? Time.fixedUnscaledDeltaTime : Time.deltaTime);
		}

        protected virtual void LateUpdate() {
			#if UNITY_EDITOR
			if (!Application.isPlaying && UnityEditor.Selection.activeGameObject != gameObject) return;
			#endif
			if (executionLoop != MonoBehaviourExecutionLoop.LateUpdate && Application.isPlaying) return;
            Execute(_ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
		}

		#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected() {
			if (_transformToFollow == null) {
				Handles.Label(transform.position, $"Follow Target is null!");
				return;
			}
			Handles.color = Gizmos.color = Color.cyan;
			var targetPos = Vector3.Scale(_transformToFollow.position, _positionMultiplier);
			if (!_followOffset.CIsZero()) {
				targetPos += _transformToFollow.TransformVector(_followOffset);
			}
			Handles.Label(targetPos, $"Follow Target: {_transformToFollow.name}");
			Gizmos.DrawLine(transform.position, targetPos);
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- General ---------->>

        protected virtual void Execute(float deltaTime) {
            FollowTarget(deltaTime);
        }

        void CheckIfWillMove() {
			if (_ignoreXAxis && _ignoreYAxis && _ignoreZAxis) {
				Debug.LogError($"'{name}' is set to ignore all axis when following so it will remain stationary.");
			}
		}

		void FollowTarget(float deltaTime) {
			if (_transformToFollow == null) return;

			if (_followRotation) {
				transform.rotation = _transformToFollow.rotation;
			}

			if (_ignoreXAxis && _ignoreYAxis && _ignoreZAxis) return;

			var targetPos = Vector3.Scale(_transformToFollow.position, _positionMultiplier);
            if (!_followOffset.CIsZero()) {
                targetPos += _transformToFollow.TransformVector(_followOffset);
            }
			if (_ignoreXAxis) targetPos.x = transform.position.x;
			if (_ignoreYAxis) targetPos.y = transform.position.y;
			if (_ignoreZAxis) targetPos.z = transform.position.z;

			switch (_followType) {
				case FollowTypeEnum.instant:
					_myTransform.position = targetPos;
					break;
				case FollowTypeEnum.smooth:
					_myTransform.position = Vector3.Lerp(_myTransform.position, targetPos, _followSpeed * deltaTime);
					break;
			}

		}

        public void SetTransformToFollow(Transform t) {
            if (_transformToFollow == t) return;
            _transformToFollow = t;
            TransformToFollowChanged?.Invoke(t);
            CheckIfWillMove();
        }

		[Button("Set Current Position as Follow Offset")]
		public void SetCurrentPositionAsFollowOffset() {
			if (_transformToFollow == null) return;
			_followOffset = _transformToFollow.InverseTransformVector(transform.position - Vector3.Scale(_transformToFollow.position, _positionMultiplier));
			#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
			#endif
		}

		[Button("Snap to Target")]
		public void SnapToTarget() {
			if (_transformToFollow == null) return;

			if (_followRotation) {
				transform.rotation = _transformToFollow.rotation;
			}

			var targetPos = Vector3.Scale(_transformToFollow.position, _positionMultiplier);
			if (!_followOffset.CIsZero()) {
				targetPos += _transformToFollow.TransformVector(_followOffset);
			}
			if (_ignoreXAxis) targetPos.x = transform.position.x;
			if (_ignoreYAxis) targetPos.y = transform.position.y;
			if (_ignoreZAxis) targetPos.z = transform.position.z;

			transform.position = targetPos;
			#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
			#endif
		}
		
		#endregion <<---------- General ---------->>
		
	}
}
