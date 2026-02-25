using System;
using UnityEngine;

namespace EnigmaCore
{
    public class OnDestroyTrigger : MonoBehaviour
    {
        public event Action OnDestroyEvent = delegate { };

        void OnDestroy()
        {
            OnDestroyEvent.Invoke();
        }
    }
}