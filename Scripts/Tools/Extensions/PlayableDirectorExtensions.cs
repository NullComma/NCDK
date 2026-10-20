using UnityEngine.Playables;

namespace NCDK {
	public static class PlayableDirectorExtensions {

		public static PlayableDirector SetAsActiveAndPlay(this PlayableDirector p) {
			if (p == null) return p;
			p.gameObject.SetActive(true);
			p.Play();
			return p;
		}

		/// <summary>
		/// See how this method is implemented to see if it fits the condition need to consider that isPlaying. 
		/// </summary>
		public static bool IsPlaying(this PlayableDirector playableDirector)
		{
			return playableDirector != null && playableDirector.state == PlayState.Playing;
		}

		/// <summary>
		/// True while the director is still producing its timeline: playing, or paused because the
		/// game itself is paused. Use this for "wait until the cutscene ends" loops — <see cref="IsPlaying"/>
		/// reports a paused director as not playing, which makes a wait exit while the cutscene is
		/// merely frozen.
		/// </summary>
		public static bool IsRunning(this PlayableDirector playableDirector)
		{
			if (playableDirector == null) return false;
			if (playableDirector.state == PlayState.Playing) return true;

			// Unity also reports Paused for a director that simply is not playing (never started,
			// or already finished), so a paused state only counts while the game is paused.
			return playableDirector.state == PlayState.Paused && ETime.IsPaused;
		}
		
	}
}
