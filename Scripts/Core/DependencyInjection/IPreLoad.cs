namespace EnigmaCore.DependencyInjection
{
    /// <summary>
    /// Makes an MonoBehaviour respond to Pre-Load events after scene loading on game objects that are not yet Awake.
    /// </summary>
    public interface IPreLoad
    {
        void OnPreLoad();
    }
}