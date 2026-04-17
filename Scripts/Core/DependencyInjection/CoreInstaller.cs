using UnityEngine;
using UnityEngine.Scripting;

namespace NullCore
{
    [Preserve]
    [DefaultExecutionOrder(int.MinValue)]
    internal class CoreInstaller : IInstaller
    {
        public int Priority => int.MinValue;
        
        public void Install()
        {
            var blockingEventsManager = new BlockingEventsManager();
            ServiceLocator.Register(blockingEventsManager);
            ServiceLocator.Register(new CursorManager(blockingEventsManager));
            ServiceLocator.RegisterLazy<TimePauseManager>();
            ServiceLocator.RegisterLazy(() => GetUISoundsBankSO());
            ServiceLocator.RegisterLazy(() => GameObjectCreate.WithComponent<Fader>("Fader"));
        }

        static UISoundsBankSO GetUISoundsBankSO()
        {
            // We avoid WaitForCompletion to prevent deadlocks on startup.
            // If the Bank is needed before Addressables are ready, a fallback is returned.
            var fallback = ScriptableObject.CreateInstance<UISoundsBankSO>();
            fallback.name = "UISoundsBankSO_Fallback";
            return fallback;
        }
    }
}