using UnityEngine.Playables;

namespace EnigmaCore
{
    public static class PlayableDirectorExtensions
    {
        /// <summary>
        /// Condition is considered using playing time if bigger than 0f and minor than its total duration.
        /// </summary>
        public static bool IsPlaying(this PlayableDirector playableDirector)
        {
            return playableDirector.time > 0f && playableDirector.time <= playableDirector.duration;
        }
    }
}