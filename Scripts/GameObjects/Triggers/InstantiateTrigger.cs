using UnityEngine;

namespace EnigmaCore { 
    public class InstantiateTrigger : MonoBehaviour
    {
        [SerializeField] GameObject _prefabToInstantiate;
        [SerializeField] Transform _parentTransform;
        [SerializeField] bool _triggerOnAwake;

        void Awake()
        {
            if (_triggerOnAwake)
            {
                Trigger();
            }
        }

        public void Trigger()
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