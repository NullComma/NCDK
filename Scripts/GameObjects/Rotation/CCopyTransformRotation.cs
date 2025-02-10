using UnityEngine;

namespace EnigmaCore {
    [DefaultExecutionOrder(351)]
    public class CCopyTransformRotation : MonoBehaviourUpdateExecutionLoopTime {

        public Transform TransformToCopy {
            get {
                return this._transformToCopy;
            }
            set {
                this._transformToCopy = value;
            }
        }
        [SerializeField] private Transform _transformToCopy;
        
        
        
        
        protected override void Execute(float deltaTime) {
            if (TransformToCopy == null) return;
            this.transform.rotation = TransformToCopy.transform.rotation;
        }
    }
}