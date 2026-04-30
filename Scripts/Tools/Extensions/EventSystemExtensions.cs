using UnityEngine;
using UnityEngine.EventSystems;

namespace NCDK {
	public static class EventSystemExtensions {
		
		public static EventSystem GetFirstActiveEventSystem()
		{
			return Object.FindFirstObjectByType<EventSystem>();
		}
		
	}
}
