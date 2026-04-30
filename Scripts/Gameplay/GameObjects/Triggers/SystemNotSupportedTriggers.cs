using UnityEngine;
using UnityEngine.Events;

namespace NCDK {
    public class SystemNotSupportedTriggers : MonoBehaviour {
        
        [Header("Event will be invoked if feature is not supported")]
        [Space]
        [SerializeField]
        UnityEvent _tessellationShaders;
        [SerializeField] UnityEvent _computeShaders;
        [SerializeField] UnityEvent _geometryShaders;


        void Awake() {
            if(!SystemInfo.supportsTessellationShaders) _tessellationShaders?.Invoke();
            if(!SystemInfo.supportsGeometryShaders) _geometryShaders?.Invoke();
            if(!SystemInfo.supportsComputeShaders) _computeShaders?.Invoke();
        }
    }
}