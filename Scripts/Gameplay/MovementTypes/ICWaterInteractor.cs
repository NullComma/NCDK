using UnityEngine;

namespace EnigmaCore {
    public interface ICWaterInteractor {
        void OnEnterWater(Transform waterTransform);
        void OnExitWater(Transform waterTransform);
    }
}