using System.Linq;
using EnigmaCore.UI;
using Game;
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
            DIContainer.Register(typeof(PauseManager));
            DIContainer.Register(typeof(CursorManager));
            DIContainer.Register(GetUISoundsBankSO());
            DIContainer.Register(GameObjectCreate.WithComponent<CFader>("Fader"));
            DIContainer.Register(typeof(ViewManager));
            DIContainer.Register(GameObjectCreate.WithComponent<TruePlaytimeCounter>("System_PlaytimeCounter"));
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