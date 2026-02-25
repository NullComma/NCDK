using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnigmaCore {
	public class BlockingEventsManager {
                
		public bool InMenuOrPlayingCutscene => IsInMenu || IsPlayingCutscene;

        public bool IsPlayingCutscene => PlayingCutsceneRetainable.IsRetained;
        public readonly Retainable PlayingCutsceneRetainable;

        public bool IsInMenu => MenuRetainable.IsRetained;
        public readonly Retainable MenuRetainable;

        public event Action<bool> InMenuOrPlayingCutsceneEvent = delegate { };


        public BlockingEventsManager()
        {
	        MenuRetainable = new ();
	        PlayingCutsceneRetainable = new ();
	        
	        // on menu
	        MenuRetainable.StateEvent += onMenu => {
		        Debug.Log($"<color=#4fafb6>{nameof(onMenu)}: <b>{onMenu}</b></color>");
		        InMenuOrPlayingCutsceneEvent.Invoke(InMenuOrPlayingCutscene);
	        };

	        // playing cutscene
	        PlayingCutsceneRetainable.StateEvent += isPlayingCutscene => {
		        Debug.Log($"<color=#cc5636>{nameof(isPlayingCutscene)}: <b>{isPlayingCutscene}</b></color>");
		        InMenuOrPlayingCutsceneEvent.Invoke(InMenuOrPlayingCutscene);
	        };
        }

	}
}