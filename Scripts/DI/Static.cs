using System.Linq;
using UnityEngine;

namespace EnigmaCore
{
    [DefaultExecutionOrder(int.MinValue)]
    public static class Static
    {
        public static CBlockingEventsManager BlockingEventsManager { get; set; }
        public static CCursorManager CursorManager { get; set; }
        public static CLoadingCanvas LoadingCanvas { get; set; }
        public static CInputManager InputManager { get; set; }
        public static UISoundsBankSO UISoundsBankSO { get; set; }

        static void RegisterMembers()
        {
            BlockingEventsManager = new CBlockingEventsManager();
            InputManager = new CInputManager();
            CursorManager = new CCursorManager();
            LoadingCanvas = CAssets.LoadResourceAndInstantiate<CLoadingCanvas>("System/Loading Canvas");
            UISoundsBankSO = GetUISoundsBankSO();
            UISoundsBankSO GetUISoundsBankSO()
            {
                var all = Resources.LoadAll<UISoundsBankSO>("");
                #if UNITY_EDITOR
                if(!all.CIsNullOrEmpty()) return all.First();
                return CScriptableObjectExtensions.EditorCreateInResourcesFolder<UISoundsBankSO>();
                #endif
                return all.First();
            }
        }

        static void DisponseMembers()
        {
            BlockingEventsManager = null;
            CursorManager = null;
            InputManager = null;
            LoadingCanvas.CDestroyGameObject();
            Resources.UnloadAsset(UISoundsBankSO);
            UISoundsBankSO = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            //Debug.Log("Initializing Static members");
            RegisterMembers();
            CApplication.QuittingEvent += OnQuittingEvent;
        }

        static void OnQuittingEvent()
        {
            //Debug.Log("Disposing Static members");
            DisponseMembers();
            CApplication.QuittingEvent -= OnQuittingEvent;
        }
    }
}