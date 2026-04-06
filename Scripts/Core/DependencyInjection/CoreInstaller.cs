using System.Linq;
using NullCore.UI;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.AddressableAssets;

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
            var handle = Addressables.LoadResourceLocationsAsync("UISoundsBankSO");
            var locations = handle.WaitForCompletion();
            
            if (locations != null && locations.Count > 0)
            {
                var assetHandle = Addressables.LoadAssetAsync<UISoundsBankSO>(locations[0]);
                var asset = assetHandle.WaitForCompletion();
                if (asset != null) return asset;
            }
            
            var fallback = ScriptableObject.CreateInstance<UISoundsBankSO>();
            fallback.name = "UISoundsBankSO_Fallback";
            return fallback;
        }
    }
}