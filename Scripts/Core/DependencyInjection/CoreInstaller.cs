using System.Linq;
using EnigmaCore.UI;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.AddressableAssets;

namespace EnigmaCore.DependencyInjection
{
    [Preserve]
    [DefaultExecutionOrder(int.MinValue)]
    internal class CoreInstaller : IInstaller
    {
        public int Priority => int.MinValue;
        
        public void Install()
        {
            DIContainer.RegisterLazy(typeof(BlockingEventsManager));
            DIContainer.RegisterLazy(typeof(TimePauseManager));
            DIContainer.Register(typeof(CursorManager));
            DIContainer.RegisterLazy(() => GetUISoundsBankSO());
            DIContainer.RegisterLazy(() => GameObjectCreate.WithComponent<Fader>("Fader"));
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