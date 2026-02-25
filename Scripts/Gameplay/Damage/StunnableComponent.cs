using System;
using System.Collections;
using EnigmaCore.Enums;
using UnityEngine;

namespace EnigmaCore {
	/// <summary>
	/// Game object with this component can be stunned.
	/// </summary>
	[RequireComponent(typeof(HealthComponent))]
	public class StunnableComponent : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		
		// References
		[NonSerialized] HitInfoData _lastAttackData;
		[NonSerialized] HealthComponent _health;
		
		// Animation
		[NonSerialized] Animator _animator;
		[NonSerialized] int ANIM_LIGHTSTUN = Animator.StringToHash("stunLight");
		[NonSerialized] int ANIM_MEDIUMSTUN = Animator.StringToHash("stunMedium");
		[NonSerialized] int ANIM_HEAVYSTUN = Animator.StringToHash("stunHeavy");
		
		// Stun
		const float LIGHT_STUN_FRACTION = 0.20f;
		const float MEDIUM_STUN_FRACTION = 0.60f;
		[NonSerialized] bool _stunned;
		[SerializeField, Min(0f)] float _stunRecoveryRatePerSecond = 0.3f;
		[SerializeField] float _heavyStunResistance = 50f;
		
		public StunStatus StunStatus {
			get { return _stunStatus; }
			private set {
				if (_stunStatus == value) return;
				_stunStatus = value;

				switch (_stunStatus) {
					case StunStatus.lightStun: {
						_animator.CSetTriggerSafe(ANIM_LIGHTSTUN);
						break;
					}
					case StunStatus.mediumStun: {
						_animator.CSetTriggerSafe(ANIM_MEDIUMSTUN);
						break;
					}
					case StunStatus.heavyStun: {
						_animator.CSetTriggerSafe(ANIM_HEAVYSTUN);
						break;
					}
				}

			}
		}
		StunStatus _stunStatus;

		float StunProgress {
			get { return _stunProgress; }
			set {
				if (_stunProgress == value) return;
				_stunProgress = value;
				if (_stunProgress < 0) {
					_stunProgress = 0f;
					return;
				}

				float stunPercentage = _stunProgress / _heavyStunResistance;

				if (stunPercentage >= 1f) {
					StunStatus = StunStatus.heavyStun;
				}else if (stunPercentage >= MEDIUM_STUN_FRACTION) {
					StunStatus = StunStatus.mediumStun;
				}else if (stunPercentage >= LIGHT_STUN_FRACTION) {
					StunStatus = StunStatus.lightStun;
				}
				else {
					StunStatus = StunStatus.none;
				}
				
			}
		}
		[NonSerialized] float _stunProgress;
		
		#endregion <<---------- Properties and Fields ---------->>

		

		
		#region <<---------- MonBehaviour ---------->>

		void Awake() {
			_health = GetComponent<HealthComponent>();
			_animator = GetComponent<Animator>();
		}

		void OnEnable() {
			_health.OnDamageTaken += DamageTake;
			_health.OnRevive += Revived;
			this.CStartCoroutine(StunRecoveryRoutine());
		}

		void OnDisable() {
			_health.OnDamageTaken -= DamageTake;
			_health.OnRevive -= Revived;
		}
		
		#endregion <<---------- MonBehaviour ---------->>

		IEnumerator StunRecoveryRoutine()
		{
			while (enabled) {
				yield return new WaitForSeconds(_stunRecoveryRatePerSecond);
				if (_health.IsDead) continue;
				StunProgress -= _stunRecoveryRatePerSecond;
			}
		}

		void DamageTake(float dmgAmount, HitInfoData attack, Transform attacker) {
			StunProgress += dmgAmount;
			_lastAttackData = attack;
		}

		void Revived() {
			_stunProgress = 0f;
		}

	}
}
