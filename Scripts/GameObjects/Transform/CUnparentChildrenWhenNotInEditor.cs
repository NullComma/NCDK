using UnityEngine;

namespace EnigmaCore {
    public class CUnparentChildrenWhenNotInEditor : CUnparentChildrenOnAwake {
        protected override void Awake() {
            if (Application.isEditor) return;
            base.Awake();
        }
    }
}