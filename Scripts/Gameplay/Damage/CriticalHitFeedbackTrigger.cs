using System;
using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore {
    public class CriticalHitFeedbackTrigger : MonoBehaviour {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void ResetListeners() {
            OnCriticalHit = null;
        }

        [SerializeField] UnityEvent _onCriticalHit;
        public static event EventHandler<HealthComponent> OnCriticalHit = delegate { };

        void OnEnable() {
            OnCriticalHit += CriticalHitCallback;
        }

        void CriticalHitCallback(object sender, HealthComponent healthHit) {
            if (healthHit.CompareTag("Player")) return;
            _onCriticalHit?.Invoke();
        }

        void OnDisable() {
            OnCriticalHit -= CriticalHitCallback;
        }
    }
}