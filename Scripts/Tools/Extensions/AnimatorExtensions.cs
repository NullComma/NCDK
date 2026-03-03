using System.Linq;
using UnityEngine;

namespace EnigmaCore {
	public static class AnimatorExtensions {

        /// <summary>
        /// Check if self is null and if was the parameter.
        /// </summary>
		public static void SetFloatWithLerp(this Animator self, int id, float target, float time) {
			if (!CheckIfIsAvailable(self)) return;
            if (self.parameters.All(p => p.nameHash != id)) return;
			target = target.Imprecise();
			var currentFloat = self.GetFloat(id);
			self.SetFloat(id, currentFloat.Lerp(target, time).Imprecise());
		}

        /// <summary>
        /// Check if self is null and if was the parameter.
        /// </summary>
		public static void SetBoolSafe(this Animator self, int id, bool value) {
			if (!CheckIfIsAvailable(self)) return;
            if (self.parameters.All(p => p.nameHash != id)) return;
			self.SetBool(id, value);
		}
		
		/// <summary>
		/// Check if self is null and if was the parameter.
		/// </summary>
		public static void SetFloatSafe(this Animator self, int id, float value) {
			if (!CheckIfIsAvailable(self)) return;
            if (self.parameters.All(p => p.nameHash != id)) return;
			self.SetFloat(id, value.Imprecise());
		}
        
        /// <summary>
        /// Check if self is null and if was the parameter.
        /// </summary>
        public static void SetIntegerSafe(this Animator self, int id, int value) {
            if (!CheckIfIsAvailable(self)) return;
            if (self.parameters.All(p => p.nameHash != id)) return;
            self.SetInteger(id, value);
        }

        
        /// <summary>
        /// Check if self is null and if was the parameter.
        /// </summary>
        public static void SetTriggerSafe(this Animator self, int id) {
            if (!CheckIfIsAvailable(self)) return;
			if (self.parameters.All(p => p.nameHash != id)) return;
            self.SetTrigger(id);
        }

		private static bool CheckIfIsAvailable(Animator self) {
			return self != null && self.isActiveAndEnabled;
		}

    }
}
