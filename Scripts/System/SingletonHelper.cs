using UnityEngine;

namespace NullCore {
    public static class SingletonHelper {
        
        public static T CreateInstance<T>(string gameObjectName) where T : MonoBehaviour {
            if (CannotCreateAnyInstance()) {
                return null;
            }
            return new GameObject(gameObjectName).DontDestroyOnLoad().AddComponent<T>();
        }
        
        public static bool CannotCreateAnyInstance() {
            return EApplication.IsQuitting || !Application.isPlaying;
        }

    }
}