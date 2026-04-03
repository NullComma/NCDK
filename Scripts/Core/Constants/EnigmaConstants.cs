using UnityEngine;

namespace NullCore {
	public static class EnigmaConstants {
		public const string EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX = "ScriptableObject/";

        public static WaitForEndOfFrame WaitForEndOfFrame => _waitForEndOfFrame;
        private static WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

	}
}
