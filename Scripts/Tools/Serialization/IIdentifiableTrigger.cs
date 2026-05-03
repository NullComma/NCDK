namespace NCDK
{
    /// <summary>
    /// Extends <see cref="IIdentifiableObject"/> with a trigger action,
    /// unifying identifiable objects that can be programmatically activated.
    /// </summary>
    public interface IIdentifiableTrigger : IIdentifiableObject
    {
        /// <summary>
        /// Executes the trigger behavior.
        /// </summary>
        void Trigger();
    }
}
