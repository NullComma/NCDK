using System.Linq;
using UnityEngine;

namespace EnigmaCore.DependecyInjection
{
    [DefaultExecutionOrder(int.MinValue)]
    static class CompositionRoot
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitializeSubsystemRegistration()
        {
            DIContainer.Register(typeof(CBlockingEventsManager));
            DIContainer.Register(typeof(CursorManager));
            DIContainer.Register(GetUISoundsBankSO());
            DIContainer.Register(GameObjectCreate.WithComponent<CFader>("Fader"));
        }

        static UISoundsBankSO GetUISoundsBankSO()
        {
            var all = Resources.LoadAll<UISoundsBankSO>("");
            #if UNITY_EDITOR
            if(!all.CIsNullOrEmpty()) return all.First();
            return CScriptableObjectExtensions.EditorCreateInResourcesFolder<UISoundsBankSO>();
            #endif
            return all.First();
        }
    }
}