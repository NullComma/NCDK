using System;
using NullCore.Data;
using NullCore.Damage;
using UnityEngine;
using UnityEngine.Serialization;

namespace NullCore {
	public class Hitbox : MonoBehaviour, ICDamageable {

		public const float CriticalMultiplier = 1.5f;

		public virtual HealthComponent Health => _healthToNotify;
		[SerializeField] HealthComponent _healthToNotify;
		[FormerlySerializedAs("_takeDamageEvent")] [SerializeField] private CUnityEvent _takeHitEvent;
		public bool IsCriticalHitbox => _isCriticalHitbox;
		[SerializeField] bool _isCriticalHitbox;

		
		
		public event Action<HitInfoData> OnHit {
			add {
				_onHit += value;
			}
			remove {
				_onHit -= value;
				_onHit += value;
			}
		}
		private Action<HitInfoData> _onHit;

		
		
		
		public virtual float TakeHit(HitInfoData attack, Transform attacker, float damageMultiplier) {
			_onHit?.Invoke(attack);
			float damage = attack.RawDamage;
			if (Health != null) {
				damage = Health.TakeHit(attack, attacker, GetDamageMultiplierForHit(attack) * damageMultiplier);
			}
			_takeHitEvent?.Invoke();
			return damage;
		}

		float GetDamageMultiplierForHit(HitInfoData attack) {
			if (!attack.CanCritical) return 1f;
			return _isCriticalHitbox ? CriticalMultiplier : 1f;
		}
	}
}