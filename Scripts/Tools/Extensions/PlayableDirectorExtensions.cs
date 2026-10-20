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
		/// game was paused. Use this for "wait until the cutscene ends" loops — <see cref="IsPlaying"/>
		/// reports a paused director as not playing, which makes a wait exit while the cutscene is
		/// merely frozen.
		/// </summary>
		public static bool IsRunning(this PlayableDirector playableDirector)
		{
			return playableDirector != null
			       && (playableDirector.state == PlayState.Playing || playableDirector.state == PlayState.Paused);
		}
		
	}
}
