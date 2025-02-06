using UnityEngine;

namespace EnigmaCore {
    public class CUnparentChildrenOnAwake : MonoBehaviour {
        protected virtual void Awake() {
            Unparent();
        }

        void Unparent() {
            transform.SetAsLastSibling();
            transform.UnparentAllChildren();
        }
    }
}