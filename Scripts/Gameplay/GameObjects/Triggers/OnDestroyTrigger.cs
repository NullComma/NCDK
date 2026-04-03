using System;
using UnityEngine;

namespace NullCore
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