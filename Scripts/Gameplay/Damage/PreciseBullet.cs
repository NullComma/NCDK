using System;
using NullCore.Damage;
using UnityEngine;

namespace NullCore {
    public class PreciseBullet : DamageDealerTrigger {

        [Range(1, 20), SerializeField] float autoDestroyAfter = 3f;
        [SerializeField] float moveSpeed = 22f;
        [SerializeField] float bulletSize = 0.05f;
        [SerializeField] LayerMask hitLayers;
        Vector3 previousPosition;

        void Start() {
            this.DestroyGameObject(autoDestroyAfter);
        }

        void Update() {
            previousPosition = transform.position;
            transform.position += transform.forward * (moveSpeed * Time.deltaTime);
        }

        void LateUpdate() {
            if (!Physics.SphereCast(previousPosition,
                    bulletSize,
                    transform.forward,
                    out var hit,
                    Vector3.Distance(transform.position, previousPosition),
                    hitLayers,
                    QueryTriggerInteraction.Ignore
                )) return;
            var damageable = hit.collider.GetComponent<ICDamageable>();
            damageable?.TakeHit(AttackData.data, AttackData.AttackerTransform);
            DestroyBullet(hit.point);
        }

        void DestroyBullet(Vector3 hitPoint) {
            this.DestroyGameObject();
        }

        #if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, bulletSize);
        }
        #endif
    }
}