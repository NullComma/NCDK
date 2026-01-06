using System;
using EnigmaCore.DependencyInjection;
using UnityEngine;

namespace EnigmaCore
{
    /// <summary>
    /// Listens to blocking state changes from <see cref="CBlockingEventsManager"/> 
    /// and triggers corresponding UnityEvents for menus and cutscenes.
    /// </summary>
    public class BlockingEventsTrigger : MonoBehaviour
    {
        [NonSerialized, Inject] CBlockingEventsManager _blockingEventsManager;

        [Header("Combined (Menu OR Cutscene)")]
        [Tooltip("Triggered when the game is paused OR in a cutscene.")]
        [SerializeField] 
        StateUnityEvents _blockingState = new();

        [Header("Individual States")]
        [Tooltip("Triggered based strictly on the Menu state.")]
        [SerializeField] 
        StateUnityEvents _menuState = new();

        [Tooltip("Triggered based strictly on the Cutscene state.")]
        [SerializeField] 
        StateUnityEvents _cutsceneState = new();

        void Awake()
        {
            this.Inject();
            
            UpdateStates(_blockingEventsManager.InMenuOrPlayingCutscene);
        }

        void OnEnable()
        {
            _blockingEventsManager.InMenuOrPlayingCutsceneEvent += UpdateStates;
        }

        void OnDisable()
        {
            _blockingEventsManager.InMenuOrPlayingCutsceneEvent -= UpdateStates;
        }

        /// <summary>
        /// Updates all event groups based on the current manager state.
        /// </summary>
        /// <param name="isBlocking">Current global blocking state (Menu or Cutscene).</param>
        void UpdateStates(bool isBlocking)
        {
            var inMenu = _blockingEventsManager.IsInMenu;
            var inCutscene = _blockingEventsManager.IsPlayingCutscene;

            // Trigger combined events (Menu OR Cutscene)
            _blockingState.Trigger(isBlocking);

            // Trigger menu-specific events
            _menuState.Trigger(inMenu);

            // Trigger cutscene-specific events
            _cutsceneState.Trigger(inCutscene);
        }
    }
}