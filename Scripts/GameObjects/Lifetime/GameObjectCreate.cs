using UnityEngine;

namespace EnigmaCore
{
    public static class GameObjectCreate
    {
        public static T WithComponent<T>(string name = null, HideFlags hideFlags = default) where T : Component
        {
            var createdGo = new GameObject(name ?? typeof(T).Name);
            if(hideFlags != default) createdGo.hideFlags = hideFlags;
            return createdGo.AddComponent<T>();
        }
    }
}