using UnityEngine;

namespace EnigmaCore {
	public class ETimeTriggers : MonoBehaviour {

		[Header("Booleans")]
		[SerializeField] CUnityEventBool _timeStoppedEvent;
		[SerializeField] CUnityEventBool _timeResumedEvent;
		
		[Header("Floats")]
		[SerializeField] CUnityEventFloat _timeScaleValueEvent;


		void OnEnable() {
			ETime.OnTimeScaleChanged += TimeScaleChanged;
		}
		void OnDisable() {
			ETime.OnTimeScaleChanged -= TimeScaleChanged;
		}


		void TimeScaleChanged(float oldTimeScale, float newTimeScale) {
			bool timeStopped = newTimeScale <= 0f;
			_timeResumedEvent?.Invoke(!timeStopped);
			_timeStoppedEvent?.Invoke(timeStopped);
			
			_timeScaleValueEvent?.Invoke(newTimeScale);
		}
	}
}
