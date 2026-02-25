using UnityEngine;

namespace EnigmaCore {
    public class UnparentChildrenWhenNotInEditor : UnparentChildrenOnAwake {
        protected override void Awake() {
            if (Application.isEditor) return;
            base.Awake();
        }
    }
}