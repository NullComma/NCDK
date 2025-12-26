using System;
using EnigmaCore.DependecyInjection;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnigmaCore
{
    public class CBlockingEventsTrigger : MonoBehaviour
    {
        [NonSerialized, Inject] CBlockingEventsManager _blockingEventsManager;

        // --- Combined 
#if ODIN_INSPECTOR
        [FoldoutGroup("Default")]
#else
        [Header("Default")]
#endif
        [SerializeField]
        CUnityEventBool AnyBlockingEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Default")]
#endif
        [SerializeField]
        CUnityEventBool OnMenuOrPlayingCutsceneEvent = new();

        // --- Individual 
#if ODIN_INSPECTOR
        [FoldoutGroup("Individual")]
#else
        [Header("Individual")]
#endif
        [SerializeField]
        CUnityEventBool OnMenuEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Individual")]
#endif
        [SerializeField]
        CUnityEventBool PlayingCutsceneEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Individual")]
#endif
        [SerializeField]
        CUnityEventBool PlayingCutsceneInvertedEvent = new();

        // --- Inverted 
#if ODIN_INSPECTOR
        [FoldoutGroup("Inverted")]
#else
        [Header("Inverted")]
#endif
        [SerializeField]
        CUnityEventBool NotOnMenuEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Inverted")]
#endif
        [SerializeField]
        CUnityEventBool NotPlayingCutsceneEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Inverted")]
#endif
        [SerializeField]
        CUnityEventBool NotOnMenuAndNotPlayingCutsceneEvent = new();

        void Awake()
        {
            this.Inject();
            BlockingEvent(_blockingEventsManager.InMenuOrPlayingCutscene);
        }

        void OnEnable()
        {
            _blockingEventsManager.InMenuOrPlayingCutsceneEvent += BlockingEvent;
        }

        void OnDisable()
        {
            _blockingEventsManager.InMenuOrPlayingCutsceneEvent -= BlockingEvent;
        }

        void BlockingEvent(bool inMenuOrPlayingCutscene)
        {
            var isInMenu = _blockingEventsManager.IsInMenu;
            var isPlayingCutscene = _blockingEventsManager.IsPlayingCutscene;

            // inverted
            NotOnMenuEvent.Invoke(!isInMenu);
            NotPlayingCutsceneEvent.Invoke(!isPlayingCutscene);
            NotOnMenuAndNotPlayingCutsceneEvent.Invoke(!isInMenu && !isPlayingCutscene);
            
            AnyBlockingEvent.Invoke(inMenuOrPlayingCutscene);
            OnMenuOrPlayingCutsceneEvent.Invoke(isInMenu || isPlayingCutscene);
            OnMenuEvent.Invoke(isInMenu);
            PlayingCutsceneEvent.Invoke(isPlayingCutscene);
            PlayingCutsceneInvertedEvent.Invoke(!isPlayingCutscene);
        }
    }
}