using UnityEngine;

namespace NCDK {
    public class UnparentChildrenOnAwake : MonoBehaviour {
        protected virtual void Awake() {
            Unparent();
        }

        void Unparent() {
            transform.SetAsLastSibling();
            transform.UnparentAllChildren();
        }
    }
}