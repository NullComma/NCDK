using System;
using UnityEngine;

namespace EnigmaCore {
    [Obsolete("Use PlayableHelper instead")]
	public class CPlayingCutsceneSetter : MonoBehaviour {

		[SerializeField] bool _setOnEnableDisable = true;


		void OnEnable() {
			if (!_setOnEnableDisable) return;
            Static.BlockingEventsManager.PlayingCutsceneRetainable.Retain(this);
		}

		void OnDisable() {
			if (!_setOnEnableDisable) return;
			Static.BlockingEventsManager.PlayingCutsceneRetainable.Release(this);
		}

		public void SetPlayingState(bool isPlaying) {
			if(isPlaying) Static.BlockingEventsManager.PlayingCutsceneRetainable.Retain(this);
            else Static.BlockingEventsManager.PlayingCutsceneRetainable.Release(this);
		}
	}
}
