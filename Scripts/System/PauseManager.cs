using System;

namespace EnigmaCore
{
    public class PauseManager
    {
        static CRetainable pauseRetainable;

        public PauseManager()
        {
            pauseRetainable = new();
            pauseRetainable.StateEvent += PauseStateChanged;
        }

        ~PauseManager()
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