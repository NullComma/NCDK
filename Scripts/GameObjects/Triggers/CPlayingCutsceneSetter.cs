using System;
using EnigmaCore.DependecyInjection;
using UnityEngine;

namespace EnigmaCore {
    [Obsolete("Use PlayableHelper instead")]
	public class CPlayingCutsceneSetter : MonoBehaviour {

		[SerializeField] bool _setOnEnableDisable = true;


		void OnEnable() {
			if (!_setOnEnableDisable) return;
            DIContainer.Resolve<CBlockingEventsManager>().PlayingCutsceneRetainable.Retain(this);
		}

		void OnDisable() {
			if (!_setOnEnableDisable) return;
			DIContainer.Resolve<CBlockingEventsManager>().PlayingCutsceneRetainable.Release(this);
		}

		public void SetPlayingState(bool isPlaying) {
			if(isPlaying) DIContainer.Resolve<CBlockingEventsManager>().PlayingCutsceneRetainable.Retain(this);
            else DIContainer.Resolve<CBlockingEventsManager>().PlayingCutsceneRetainable.Release(this);
		}
	}
}
