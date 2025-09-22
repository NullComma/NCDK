using System;
using UnityEngine;

namespace EnigmaCore.GameObjects {
    public class CSetScaleTrigger : MonoBehaviour {

        [SerializeField, ReadOnly] Vector3 initialScale;
        [SerializeField] Vector3 _targetLocalScale = Vector3.one;

        void OnDrawGizmosSelected()
        {
            if(Application.isPlaying) return;
            initialScale = transform.localScale;
        }

        public void ResetScale() {
            transform.localScale = initialScale;
        }

        public void SetToTargetScale()
        {
            transform.localScale = _targetLocalScale;
        }
        
        public void SetScale(Vector3 newScale) {
            transform.localScale = newScale;
        }
        
    }
}