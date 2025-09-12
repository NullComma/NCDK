using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace EnigmaCore
{
    public class ETimedLoopStateTrigger : MonoBehaviour
    {
		[CMinMaxSlider(minRange, maxRange)]
		[SerializeField] Vector2 _timeRangeOn = new (1f,1f);
		[CMinMaxSlider(minRange, maxRange)]
		[SerializeField] Vector2 _timeRangeOff = new (1f,1f);

		const float minRange = 0.01f;
		const float maxRange = 60f;
		
	    [NonSerialized] bool state;

		[SerializeField] UnityEvent _stateOnEvent = new ();
		[SerializeField] UnityEvent _stateOffEvent = new ();
		[SerializeField] CUnityEventBool _stateEvent = new ();
		[SerializeField] CUnityEventBool _stateInvertedEvent = new ();
		
	    Coroutine loopRoutine;
	    
        void OnEnable()
		{
			TriggerStateEvents();
			loopRoutine = this.CStartCoroutine(LoopRoutine());
		}

		void OnDisable()
		{
			this.CStopCoroutine(loopRoutine);
			state = false;
			TriggerStateEvents();
		}

		IEnumerator LoopRoutine()
		{
			while (enabled)
            {
                state = !state;
                yield return new WaitForSeconds(Random.Range(
					state ? _timeRangeOn.x : _timeRangeOff.x, 
					state ? _timeRangeOn.y : _timeRangeOff.y
					));
				
				TriggerStateEvents();
            }
		}

		void TriggerStateEvents()
		{
			if (state) _stateOnEvent.Invoke();
			else _stateOffEvent.Invoke();
			_stateEvent.Invoke(state);
			_stateInvertedEvent.Invoke(!state);
		}
    }
}