using System;

namespace EnigmaCore
{
    /// <summary>
    /// Manages the game's pause state using a retain/release system to set the TimeScale.
    /// </summary>
    public class TimePauseManager
    {
        static CRetainable pauseRetainable;

        public TimePauseManager()
        {
            pauseRetainable = new();
            pauseRetainable.StateEvent += PauseStateChanged;
        }

        ~TimePauseManager()
        {
            pauseRetainable.StateEvent -= PauseStateChanged;
        }

        void PauseStateChanged(bool isPaused)
        {
            ETime.IsPaused = isPaused;
        }

        public void Retain(object source)
        {
            pauseRetainable.Retain(source);
        }

        public void Release(object source)
        {
            pauseRetainable.Release(source);
        }
	}
}