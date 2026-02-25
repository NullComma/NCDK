using UnityEngine;

namespace EnigmaCore {
    public class DisableOnEditor : MonoBehaviour {
        #if UNITY_EDITOR
        void Awake() {
            Debug.Log($"<b>Editor only</b>: Disabling {name}", this);
            gameObject.SetActive(false);
        }
        #endif
    }
}