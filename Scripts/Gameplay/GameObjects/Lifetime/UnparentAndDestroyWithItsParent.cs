using System;
using UnityEngine;

namespace EnigmaCore
{
    public class UnparentAndDestroyWithItsParent : MonoBehaviour
    {
        [NonSerialized] Transform _previousParent;
        
        void Awake()
        {
            if (!HasParent()) return;
            _previousParent = transform.parent;
            transform.parent = null;

            _previousParent.gameObject.AddComponent<OnDestroyTrigger>().OnDestroyEvent += OnPreviousParentDestroyEvent;
        }
        void OnPreviousParentDestroyEvent()
        {
            gameObject.CDestroy();
        }

        void OnValidate()
        {
            HasParent();
        }

        bool HasParent()
        {
            if (transform.parent != null) return true;
            Debug.LogError($"Object has the component '{nameof(UnparentAndDestroyWithItsParent)}' but has no parent.");
            return false;
        }
    }
}