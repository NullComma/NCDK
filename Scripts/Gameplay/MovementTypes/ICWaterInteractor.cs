using UnityEngine;

namespace NullCore {
    public interface ICWaterInteractor {
        void OnEnterWater(Transform waterTransform);
        void OnExitWater(Transform waterTransform);
    }
}