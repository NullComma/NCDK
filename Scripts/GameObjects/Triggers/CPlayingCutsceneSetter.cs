using System;
using EnigmaCore.DependencyInjection;
using UnityEngine;

namespace EnigmaCore {
    [Obsolete("Use PlayableHelper instead")]
	public class CPlayingCutsceneSetter : MonoBehaviour {
		[NonSerialized,Inject] CBlockingEventsManager _blockingEventsManager;
		[SerializeField] bool _setOnEnableDisable = true;

		void Awake() => this.Inject();

		void OnEnable() {
			if (!_setOnEnableDisable) return;
			_blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
		}

		void OnDisable() {
			if (!_setOnEnableDisable) return;
			_blockingEventsManager.PlayingCutsceneRetainable.Release(this);
		}

		public void SetPlayingState(bool isPlaying) {
			if(isPlaying) _blockingEventsManager.PlayingCutsceneRetainable.Retain(this);
            else _blockingEventsManager.PlayingCutsceneRetainable.Release(this);
		}
	}
}
