using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace NullCore {
	public static class IEnumerableExtensions {
		public static T RandomElement<T>(this IEnumerable<T> enumerable) {
			var array = enumerable as T[] ?? enumerable.ToArray();
			if (array.Length <= 0) return default;
			int index = EnigmaRandom.system.Next(0, array.Length);
			return array.ElementAt(index);
		}
		
		public static bool ContainsIndex<T>(this IEnumerable<T> enumerable, int index) {
			return index >= 0 && enumerable != null && index < enumerable.Count();
		}

		public static T GetAtIndexSafe<T>(this IEnumerable<T> enumerable, int index) {
			var array = enumerable as T[] ?? enumerable.ToArray();
			return array.ContainsIndex(index) ? array.ElementAt(index) : default;
		}

		public static bool HasAnyAndNotNull<T>(this IEnumerable<T> enumerable) {
			return enumerable != null && enumerable.Any();
		}
		
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) {
			return !HasAnyAndNotNull(enumerable);
		}

        public static void DoForEachNotNull<T>(this IEnumerable<T> enumerable, Action<T> a) where T : Object {
            if (enumerable == null || a == null) return;
            foreach (var o in enumerable) {
                if (o == null) continue;
                a.Invoke(o);
            }
        }

        public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T> enumerable) {
            if (enumerable == null) yield break;
            foreach (var o in enumerable) {
	            if (o is UnityEngine.Object uObj && uObj == null) continue;
                if (o == null) continue;
                yield return o;
            }
        }

    }
}