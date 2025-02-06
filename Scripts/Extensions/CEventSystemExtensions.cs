using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EnigmaCore {
	public static class CEventSystemExtensions {
		
		public static EventSystem GetFirstActiveEventSystem() {
			return GameObject.FindObjectsOfType<EventSystem>().FirstOrDefault(x => x.gameObject.activeInHierarchy);
		}
		
	}
}
