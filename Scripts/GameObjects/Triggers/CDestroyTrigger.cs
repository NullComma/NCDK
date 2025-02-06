using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore {
    public class CDestroyTrigger : MonoBehaviour {

        [SerializeField] UnityEvent DestroyEvent;
        
        
        public void DestroyGameObject(GameObject go) {
            go.CDestroy();
            DestroyEvent?.Invoke();
        }
        
    }
}