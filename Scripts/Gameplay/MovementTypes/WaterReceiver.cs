using UnityEngine;

namespace NCDK {
    [RequireComponent(typeof(Collider))]
    public class WaterReceiver : MonoBehaviour, ICWaterInteractor {

        [SerializeField] private CUnityEventTransform _onEnterWater;
        [SerializeField] private CUnityEventTransform _onExitWater;
        
        
        
        
        public void OnEnterWater(Transform waterTransform) {
            _onEnterWater?.Invoke(waterTransform);
        }

        public void OnExitWater(Transform waterTransform) {
            _onExitWater?.Invoke(waterTransform);
        }
    }
}