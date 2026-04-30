using System;
using UnityEngine;

namespace NCDK
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