using UnityEngine;

namespace EnigmaCore { 
    public class InstantiateTrigger : CAutoTriggerCompBase
    {
        [SerializeField] GameObject _prefabToInstantiate;
        [SerializeField] Transform _parentTransform;

        protected override void TriggerEvent()
        {
            if (_prefabToInstantiate == null)
            {
                Debug.LogError("Prefab to instantiate is null!");
                return;
            }
            Instantiate(_prefabToInstantiate, _parentTransform);
        }

    }
}