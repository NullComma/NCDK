using UnityEngine;
using UnityEngine.EventSystems;

namespace EnigmaCore {
	public static class EventSystemExtensions {
		
		public static EventSystem GetFirstActiveEventSystem()
		{
			return Object.FindFirstObjectByType<EventSystem>();
		}
		
	}
}
