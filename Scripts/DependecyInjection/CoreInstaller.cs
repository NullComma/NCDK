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
            DIContainer.Register(typeof(CBlockingEventsManager));
            DIContainer.Register(typeof(PauseManager));
            DIContainer.Register(typeof(CursorManager));
            DIContainer.Register(GetUISoundsBankSO());
            DIContainer.Register(GameObjectCreate.WithComponent<CFader>("Fader"));
            DIContainer.Register(typeof(ViewManager));
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