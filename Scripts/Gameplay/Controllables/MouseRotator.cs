using System;
using EnigmaCore.DependencyInjection;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace EnigmaCore {
	public class MouseRotator : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		
		[NonSerialized,Inject] BlockingEventsManager _blockingEventsManager;
		[SerializeField] Vector2 _rotationSpeed = Vector2.one * 0.2f;
		[SerializeField] Vector2 _rotationYRange = new Vector2(60f, 60f);
		[SerializeField] Vector2 _rotationXRange = new Vector2(60f, 30f);
        
#if ENABLE_INPUT_SYSTEM
        [SerializeField] private InputActionReference _lookAction;
#endif

		Vector3 _eulerRotation;
		Vector2 _inputLook;
		Transform _transform;
		Quaternion _initialRotation;

		#endregion <<---------- Properties and Fields ---------->>

		#region <<---------- MonoBehaviour ---------->>
		protected void Awake() {
			this.Inject();
			_transform = transform;
			_initialRotation = _transform.rotation;
		}

        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            if (_lookAction != null && _lookAction.action != null)
                _lookAction.action.Enable();
#endif
        }

		void Update() {
			if (_blockingEventsManager.InMenuOrPlayingCutscene) return;
			
#if ENABLE_INPUT_SYSTEM
            if (_lookAction != null && _lookAction.action != null)
            {
                _inputLook = _lookAction.action.ReadValue<Vector2>();
            }
            else
            {
                _inputLook = Vector2.zero;
                // Basic fallback directly from devices if no action is assigned
                if (Mouse.current != null)
                {
                    _inputLook = Mouse.current.delta.ReadValue();
                }
                if (Gamepad.current != null && _inputLook.sqrMagnitude < 0.01f)
                {
                    _inputLook = Gamepad.current.rightStick.ReadValue();
                }
            }
#else
			_inputLook = new Vector2(UnityEngine.Input.GetAxisRaw("Look X"), UnityEngine.Input.GetAxisRaw("Look Y"));
#endif

			// rotate camera
			_transform.Rotate(Vector3.up,
				_inputLook.x * _rotationSpeed.x * Time.deltaTime,
				Space.World);
			_transform.Rotate(_transform.right,
				_inputLook.y * -1 * _rotationSpeed.y * Time.deltaTime,
				Space.World);

			// clamp rotation
			_eulerRotation = _transform.eulerAngles;
			_eulerRotation.z = 0f;
			// vertical rotation
			if (_eulerRotation.x.IsInRange(_rotationXRange.x, 360 - _rotationXRange.y)) {
				_eulerRotation.x = _eulerRotation.x.GetCloserValue(_rotationXRange.x, 360 - _rotationXRange.y);
			}
			
			// horizontal rotation
			if (_eulerRotation.y.IsInRange(_rotationYRange.x, 360 - _rotationYRange.y)) {
				_eulerRotation.y = _eulerRotation.y.GetCloserValue(_rotationYRange.x, 360 - _rotationYRange.y);
			}
			_transform.eulerAngles = _eulerRotation;
		}
		#endregion <<---------- MonoBehaviour ---------->>



		public void ResetRotation() {
			_transform.rotation = _initialRotation;
		}
		
	}
}
