using UnityEngine;

namespace NullCore {
    [RequireComponent(typeof(Rigidbody))]
    public class ActivatePhysicsOnDie : HealthComponent {

        Rigidbody rb;


        protected override void Awake() {
            base.Awake();
            TryGetComponent(out rb);
            this.OnDie += OnDieEvent;
        }

        void OnDieEvent() {
            rb.isKinematic = true;
        }

        void OnDestroy() {
            this.OnDie -= OnDieEvent;
        }

        void Reset() {
            TryGetComponent(out rb);
            rb.isKinematic = true;
            MaxHealth = 1f;
        }

    }
}