using System;
using NullCore;
using UnityEngine;

namespace NullCore {
    [Obsolete("Use PlayableHelper instead")]
	public class PlayingCutsceneSetter : MonoBehaviour {
		[NonSerialized] BlockingEventsManager _blockingEventsManager;
		[SerializeField] bool _setOnEnableDisable = true;

		void OnEnable() {
			_blockingEventsManager = ServiceLocator.Resolve<BlockingEventsManager>();
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
