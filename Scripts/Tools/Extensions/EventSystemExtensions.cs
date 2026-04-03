using UnityEngine;
using UnityEngine.EventSystems;

namespace NullCore {
	public static class EventSystemExtensions {
		
		public static EventSystem GetFirstActiveEventSystem()
		{
			return Object.FindFirstObjectByType<EventSystem>();
		}
		
	}
}
