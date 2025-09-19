#if FMOD
using FMODUnity;

namespace EnigmaCore {
    public static class EventEmitterExtensions {
        
        /// <summary>
        /// Returns the path of the event reference if in editor, otherwise returns the GUID as string, because path is not available in builds.
        /// </summary>
        public static string GetPathSafe(this EventReference eventReference) {
            #if UNITY_EDITOR
            return eventReference.Path;
            #else
            return eventReference.Guid.ToString();
            #endif
        }
        
    }
}
#endif