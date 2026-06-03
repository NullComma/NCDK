#if !UNITY_6000_5_OR_NEWER
namespace UnityEngine
{
    public abstract class PlayModeScopeAutoCleanup
    {
        public abstract void Cleanup();
    }
}
#endif
