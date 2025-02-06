using UnityEngine;

namespace EnigmaCore {
    public static class CQuaternionExtensions {

        public static Quaternion CLerp(this Quaternion a, Quaternion b, float t) {
            return Quaternion.Lerp(a, b, t);
        }
        
    }
}