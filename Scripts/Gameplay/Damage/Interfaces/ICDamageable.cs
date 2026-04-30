using NCDK.Data;
using UnityEngine;

namespace NCDK.Damage {
	public interface ICDamageable {
		public HealthComponent Health { get; }
		/// <summary>
		/// Returns the amount of final damage taken.
		/// </summary>
		float TakeHit(HitInfoData attack, Transform attackerTransform, float damageMultiplier = 1f);
	}
}