using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore {
    public class CDestroyTrigger : MonoBehaviour {

        public UnityEvent DestroyEvent;
        
        public void DestroyGameObject(GameObject go) {
            go.CDestroy();
            DestroyEvent?.Invoke();
        }
        
    }
}