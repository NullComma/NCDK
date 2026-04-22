using UnityEngine;
using System.Collections.Generic;
using NullCore;

namespace NullCore.Triggers
{
    /// <summary>
    /// Trigger to enable game objects by resolving their Unique ID (SerializableGuid) at runtime.
    /// </summary>
    public class EnableObjectsTrigger : MonoBehaviour
    {
        [Header("Target Objects")]
        [Tooltip("Objects with these IDs will be set to Active (SetActive(true)) when this trigger is fired.")]
        public List<SerializableGuidReference> targets = new();

        public void Trigger()
        {
            foreach (var target in targets)
            {
                if (IdentifiableMonoBehaviour.Registry.TryGetValue(target.Value, out IdentifiableMonoBehaviour obj))
                {
                    obj.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"[{name}] EnableObjectsTrigger: Could not find object with ID: {target.Value}", this);
                }
            }
        }
    }
}
