using UnityEngine;

namespace EnigmaCore {
	public static class AvatarIKGoalExtensions {
		public static bool IsFoot(this AvatarIKGoal value) {
			return value == AvatarIKGoal.LeftFoot || value == AvatarIKGoal.RightFoot;
		}
		
		public static bool IsHand(this AvatarIKGoal value) {
			return value == AvatarIKGoal.LeftHand || value == AvatarIKGoal.RightHand;
		}
	}
}
