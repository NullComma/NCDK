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
            var resource = Resources.Load<UISoundsBankSO>("UISoundsBankSO");
            if (resource != null) return resource;
            var fallback = ScriptableObject.CreateInstance<UISoundsBankSO>();
            fallback.name = "UISoundsBankSO_Fallback";
            return fallback;
        }
    }
}