using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace NCDK
{
    public class StartTrigger : MonoBehaviour
    {
        [SerializeField] FlexibleFloat _delay = new FlexibleFloat(5f);
        [SerializeField] bool _ignoreTimescale = true;
        [SerializeField] UnityEvent Event;
        [NonSerialized] bool alreadyCalled;

        IEnumerator Start()
        {
            float delayValue = _delay.GetValue();
            if (delayValue > 0f) yield return (_ignoreTimescale ? new WaitForSecondsRealtime(delayValue) : new WaitForSeconds(delayValue));
            Event?.Invoke();
            yield break;
        }

        void OnEnable()
        {
            // For the warning, we check if there's any potential delay configured.
            bool hasPotentialDelay = _delay.ValueMode == FlexibleFloat.Mode.Range || _delay.ConstantValue > 0f;
            if (alreadyCalled && hasPotentialDelay)
            {
                Debug.LogWarning($"This {nameof(StartTrigger)} has already been called its Start() and has a delay. Its possible that the {Event.GetPersistentEventCount()} events have been not triggered.");
            }
        }

        void OnDisable()
        {
            alreadyCalled = true;
        }
    }
}