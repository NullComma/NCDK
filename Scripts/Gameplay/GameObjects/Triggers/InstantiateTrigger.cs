using UnityEngine;

namespace EnigmaCore { 
    public class InstantiateTrigger : MonoBehaviour
    {
        [SerializeField] GameObject _prefabToInstantiate;
        [SerializeField] Transform _parentTransform;

        public void TriggerInstantiate()
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