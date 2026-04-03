using UnityEngine;

namespace NullCore {
	public static class LayerMaskExtensions {
		public static bool CContains(this LayerMask self, LayerMask anotherLayer) {
			return (self & (1 << anotherLayer)) != 0;
		}
    }
}
