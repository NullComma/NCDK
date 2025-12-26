using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore
{
    public class TimedAutoTriggerRepeating : CAutoTriggerCompBase
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [MinMaxSlider(0f, 60f, true)]
#endif
        private Vector2 _repeatInterval = new Vector2(1f, 5f);

        [SerializeField]
        private UnityEvent _onRepeatTrigger = new();

        protected override void TriggerEvent()
        {
            StartCoroutine(RepeatRoutine());
        }

        private IEnumerator RepeatRoutine()
        {
            while (enabled)
            {
                float waitTime = Random.Range(_repeatInterval.x, _repeatInterval.y);
                yield return new WaitForSeconds(waitTime);

                ExecuteRepeat();
            }
        }

        protected virtual void ExecuteRepeat()
        {
            _onRepeatTrigger.Invoke();
        }
    }
}