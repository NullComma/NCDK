using UnityEngine.Playables;

namespace EnigmaCore {
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
		
	}
}
