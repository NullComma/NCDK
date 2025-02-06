using System.Linq;
using Unity.Linq;
using UnityEngine;

namespace EnigmaCore {
    public class CDestroyIfNotEditor : MonoBehaviour {
        void Awake() {
            if (Application.isEditor) return;
            DestroyItselfAndLog();
        }

        void DestroyItselfAndLog() {
            Debug.Log($"Destroying {name} and all its children: {string.Join(", ", gameObject.Children().Select(c=>c.name))}");
            gameObject.CDestroy();
        }
    }
}