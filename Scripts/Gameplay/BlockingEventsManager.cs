using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnigmaCore {
	public class BlockingEventsManager {
                
		public bool InMenuOrPlayingCutscene => IsInMenu || IsPlayingCutscene || IsPlayingInteractiveSubtitle;

        public bool IsPlayingCutscene => PlayingCutsceneRetainable.IsRetained;
        public readonly Retainable PlayingCutsceneRetainable;

        public bool IsInMenu => MenuRetainable.IsRetained;
        public readonly Retainable MenuRetainable;

        public bool IsPlayingInteractiveSubtitle => InteractiveSubtitleRetainable.IsRetained;
        public readonly Retainable InteractiveSubtitleRetainable;

        public event Action<bool> InMenuOrPlayingCutsceneEvent = delegate { };

        public BlockingEventsManager()
        {
	        MenuRetainable = new ();
	        PlayingCutsceneRetainable = new ();
            InteractiveSubtitleRetainable = new ();
	        
	        // on menu
	        MenuRetainable.StateEvent += state => {
		        InMenuOrPlayingCutsceneEvent.Invoke(InMenuOrPlayingCutscene);
	        };

	        // playing cutscene
	        PlayingCutsceneRetainable.StateEvent += state => {
		        InMenuOrPlayingCutsceneEvent.Invoke(InMenuOrPlayingCutscene);
	        };

            // playing interactive subtitle
            InteractiveSubtitleRetainable.StateEvent += state => {
                InMenuOrPlayingCutsceneEvent.Invoke(InMenuOrPlayingCutscene);
            };
        }
	}
}