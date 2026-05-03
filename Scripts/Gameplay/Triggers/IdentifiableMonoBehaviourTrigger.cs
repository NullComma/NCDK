using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Base <see cref="MonoBehaviour"/> that is both identifiable and triggerable.
    /// Subclasses override <see cref="OnTrigger"/> to define the behavior.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class IdentifiableMonoBehaviourTrigger : IdentifiableMonoBehaviour, IIdentifiableTrigger
    {
        public void Trigger() => OnTrigger();

        protected abstract void OnTrigger();
    }
}
