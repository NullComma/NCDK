using System.Collections.Generic;
using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Component that holds a list of <see cref="IdentifiableTriggerReference"/> entries
    /// and calls <see cref="IIdentifiableTrigger.Trigger"/> on every valid entry
    /// when its own <see cref="Trigger"/> method is invoked.
    /// <para>
    /// This follows a composition-over-inheritance approach: the component itself
    /// is an <see cref="IdentifiableMonoBehaviourTrigger"/> (so it is identifiable
    /// and can be chained), but its sole responsibility is to fan-out the trigger
    /// call to the configured list.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    public class TriggerAllIdentifiableObjects : IdentifiableMonoBehaviourTrigger
    {
        [SerializeField]
        [Tooltip("Objects that implement IIdentifiableTrigger. Their Trigger() will be called in list order.")]
        private List<IdentifiableTriggerReference> _targets = new();

        /// <summary>
        /// Read-only access to the target list for inspection or testing.
        /// </summary>
        public IReadOnlyList<IdentifiableTriggerReference> Targets => _targets;

        protected override void OnTrigger()
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i].Value;
                if (target != null)
                {
                    target.Trigger();
                }
                else
                {
                    Debug.LogError(
                        $"[TriggerAllIdentifiableObjects] Entry at index {i} on '{name}' is null or does not implement IIdentifiableTrigger.",
                        this);
                }
            }
        }
    }
}
