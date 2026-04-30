using UnityEngine;
using System.Collections.Generic;
using NCDK;

namespace NCDK.Triggers
{
    /// <summary>
    /// Trigger to disable game objects by resolving their Unique ID (SerializableGuid) at runtime.
    /// </summary>
    public class DisableObjectsTrigger : MonoBehaviour
    {
        [Header("Target Objects")]
        [Tooltip("Objects with these IDs will be set to Inactive (SetActive(false)) when this trigger is fired.")]
        public List<SerializableGuidReference> targets = new();

        public void Trigger()
        {
            foreach (var target in targets)
            {
                if (IdentifiableMonoBehaviour.Registry.TryGetValue(target.Value, out IdentifiableMonoBehaviour obj))
                {
                    obj.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogWarning($"[{name}] DisableObjectsTrigger: Could not find object with ID: {target.Value}", this);
                }
            }
        }
    }
}
