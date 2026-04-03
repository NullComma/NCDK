using UnityEngine;
using UnityEngine.Events;

namespace NullCore {
    public class DestroyTrigger : MonoBehaviour {

        public UnityEvent DestroyEvent;
        
        public void DestroyGameObject(GameObject go) {
            go.CDestroy();
            DestroyEvent?.Invoke();
        }
        
    }
}