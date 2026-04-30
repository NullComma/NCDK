using UnityEngine;

namespace NCDK {
    public interface ICWaterInteractor {
        void OnEnterWater(Transform waterTransform);
        void OnExitWater(Transform waterTransform);
    }
}