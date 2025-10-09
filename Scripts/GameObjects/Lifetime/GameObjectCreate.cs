using UnityEngine;

namespace EnigmaCore
{
    public static class GameObjectCreate
    {
        public static T WithComponent<T>(string name = null) where T : Component
        {
            return new GameObject(name ?? typeof(T).Name).AddComponent<T>();
        }

    }
}