using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NullCore {
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
	        MenuRetainable.StateEvent += state => {
		        InMenuOrPlayingCutsceneEvent.Invoke(InMenuOrPlayingCutscene);
	        };

	        // playing cutscene
	        PlayingCutsceneRetainable.StateEvent += state => {
		        InMenuOrPlayingCutsceneEvent.Invoke(InMenuOrPlayingCutscene);
	        };
        }
	}
}