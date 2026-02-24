using System.Linq;
using EnigmaCore.UI;
using UnityEngine;
using UnityEngine.Scripting;

namespace EnigmaCore.DependencyInjection
{
    [Preserve]
    [DefaultExecutionOrder(int.MinValue)]
    internal class CoreInstaller : IInstaller
    {
        public int Priority => int.MinValue;
        
        public void Install()
        {
            DIContainer.RegisterLazy(typeof(CBlockingEventsManager));
            DIContainer.RegisterLazy(typeof(TimePauseManager));
            DIContainer.Register(typeof(CursorManager));
            DIContainer.RegisterLazy(() => GetUISoundsBankSO());
            DIContainer.RegisterLazy(() => GameObjectCreate.WithComponent<CFader>("Fader"));
        }

        static UISoundsBankSO GetUISoundsBankSO()
        {
            var all = Resources.LoadAll<UISoundsBankSO>("");
#if UNITY_EDITOR
            if(!all.CIsNullOrEmpty()) return all.First();
            return CScriptableObjectExtensions.EditorCreateInResourcesFolder<UISoundsBankSO>();
#else
            return all.First();
#endif
        }
    }
}