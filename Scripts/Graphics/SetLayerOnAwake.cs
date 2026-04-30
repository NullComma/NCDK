using UnityEngine;

namespace NCDK {
    public class SetLayerOnAwake : MonoBehaviour {

        [SerializeField] private LayerMask _layer = -1;
        
        
        public void Awake() {
            this.gameObject.CSetLayerFromLayerMask(this._layer);
            Debug.Log($"Setting '{this.name}' to '{this.gameObject.layer.ToString()}'");
        }
    }
}