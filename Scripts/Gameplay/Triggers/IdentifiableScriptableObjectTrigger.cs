namespace NCDK
{
    /// <summary>
    /// Base <see cref="UnityEngine.ScriptableObject"/> that is both identifiable
    /// (via <see cref="IdentifiableScriptableObject"/>) and triggerable
    /// (via <see cref="IIdentifiableTrigger"/>).
    /// <para>
    /// Subclasses override <see cref="OnTrigger"/> to define what happens when the trigger fires.
    /// Because <see cref="ScriptableObject"/> instances are data-centric and may be shared,
    /// trigger logic should generally be stateless or idempotent.
    /// </para>
    /// </summary>
    public abstract class IdentifiableScriptableObjectTrigger : IdentifiableScriptableObject, IIdentifiableTrigger
    {
        /// <summary>
        /// Fires the trigger. Non-virtual so orchestration can be added here later.
        /// Subclasses implement <see cref="OnTrigger"/> instead.
        /// </summary>
        public void Trigger() => OnTrigger();

        /// <summary>
        /// Override to define the behaviour executed when this trigger is fired.
        /// </summary>
        protected abstract void OnTrigger();
    }
}
