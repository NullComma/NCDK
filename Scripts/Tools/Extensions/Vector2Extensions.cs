using UnityEngine;

namespace NCDK {
	public static class Vector2Extensions {
		public static float GetAbsBiggerValue(this Vector2 vec) {
			float biggerValue = Mathf.Abs(vec.x);
			float valueToCompare = Mathf.Abs(vec.y);
			if (valueToCompare > biggerValue) {
				biggerValue = valueToCompare;
			}

			return biggerValue;
		}
        
        public static bool ImpreciseEqualCompare(this Vector2 a, Vector2 b) {
            return a.x.Imprecise() == b.x.Imprecise()
            && a.y.Imprecise() == b.y.Imprecise();
        }
        
        public static Vector2 CastValuesToInt(this Vector2 a) {
            a.x = (int)a.x;
            a.y = (int)a.y;
            return a;
        }
        
        public static bool IsZero(this Vector2 v) {
            return v.x == 0f && v.y == 0f;
        }
        
        public static bool IsOne(this Vector2 v) {
            return v.x == 1f && v.y == 1f;
        }

        public static Vector2 Lerp(this Vector2 v, Vector2 other, float t) {
            v.x = Mathf.Lerp(v.x, other.x, t);
            v.y = Mathf.Lerp(v.y, other.y, t);
            return v;
        }

		public static float GetRandomValueInsideRange(this Vector2 v)
		{
			return Random.Range(v.x, v.y);
		}

	}
}