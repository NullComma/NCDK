using NCDK.Refs;
using UnityEngine;
using UnityEngine.Playables;
using System;

namespace NCDK {
    /// <summary>
    /// Pauses a <see cref="PlayableDirector"/> while any shared blocking state is active
    /// and resumes it automatically as soon as the blocking state clears.
    ///
    /// This component listens to the aggregated state exposed by <see cref="BlockingEventsManager"/>,
    /// so overlapping blocking sources are handled naturally: the Timeline only resumes once all
    /// blocking retainers have been released.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayableDirector))]
    public sealed class PlayableDirectorBlockingPauseController : ValidatedMonoBehaviour {
        [Header("References")]
        [Self,SerializeField] private PlayableDirector _director;

        [NonSerialized] private BlockingEventsManager _blockingEventsManager;
        [NonSerialized] private bool _isSubscribedToBlockingEvents;
        [NonSerialized] private bool _isBlocked;
        [NonSerialized] private bool _pausedByThisComponent;
        [NonSerialized] private bool _shouldResumeWhenUnblocked;
        [NonSerialized] private bool _wasPlayingBeforePause;

        private void Awake() {
            if (!_director && !TryGetComponent(out _director)) {
                Debug.LogError($"{nameof(PlayableDirectorBlockingPauseController)} requires a {nameof(PlayableDirector)} on the same GameObject.", this);
                enabled = false;
                return;
            }

            _blockingEventsManager = ServiceLocator.Resolve<BlockingEventsManager>();
            if (_blockingEventsManager == null) {
                Debug.LogError($"{nameof(PlayableDirectorBlockingPauseController)} requires a registered {nameof(BlockingEventsManager)}.", this);
                enabled = false;
                return;
            }

            SubscribeToBlockingEvents();
        }

        private void OnEnable() {
            if (_blockingEventsManager == null) {
                Debug.LogError($"{nameof(PlayableDirectorBlockingPauseController)} requires a registered {nameof(BlockingEventsManager)}.", this);
                enabled = false;
                return;
            }
            SyncWithCurrentBlockingState();
        }

        private void Update() {
            if (_director == null) return;

            if (_isBlocked) {
                PauseDirectorIfNeeded();
            }
            else {
                ResumeDirectorIfNeeded();
            }
        }

        private void OnDisable() {
            UnsubscribeFromBlockingEvents();
            UnsubscribeFromDirectorEvents();
        }

        private void OnDestroy() {
            UnsubscribeFromBlockingEvents();
            UnsubscribeFromDirectorEvents();
        }

        private void SubscribeToBlockingEvents() {
            if (_blockingEventsManager == null || _isSubscribedToBlockingEvents) return;

            _blockingEventsManager.InMenuOrPlayingCutsceneEvent += OnBlockingStateChanged;
            _isSubscribedToBlockingEvents = true;
        }

        private void UnsubscribeFromBlockingEvents() {
            if (_blockingEventsManager == null || !_isSubscribedToBlockingEvents) return;

            _blockingEventsManager.InMenuOrPlayingCutsceneEvent -= OnBlockingStateChanged;
            _isSubscribedToBlockingEvents = false;
        }

        private void SubscribeToDirectorEvents() {
            if (_director == null) return;
            _director.stopped -= OnDirectorStopped;
            _director.stopped += OnDirectorStopped;
        }

        private void UnsubscribeFromDirectorEvents() {
            if (_director == null) return;
            _director.stopped -= OnDirectorStopped;
        }

        private void SyncWithCurrentBlockingState() {
            SubscribeToDirectorEvents();
            OnBlockingStateChanged(_blockingEventsManager.InMenuOrPlayingCutscene);
        }

        private void OnBlockingStateChanged(bool isBlocked) {
            _isBlocked = isBlocked;

            if (_director == null) return;

            if (isBlocked) {
                PauseDirectorIfNeeded();
            }
            else {
                ResumeDirectorIfNeeded();
            }
        }

        private void PauseDirectorIfNeeded() {
            if (_director == null) return;
            if (_director.state != PlayState.Playing) return;
            if (_pausedByThisComponent) return;

            _wasPlayingBeforePause = true;
            _shouldResumeWhenUnblocked = true;
            _pausedByThisComponent = true;
            _director.Pause();
        }

        private void ResumeDirectorIfNeeded() {
            if (!_pausedByThisComponent) return;

            bool canResume = _shouldResumeWhenUnblocked && _wasPlayingBeforePause;
            ClearPauseTracking();

            if (!canResume || _director == null) return;
            if (_director.state != PlayState.Paused) return;

            _director.Play();
        }

        private void OnDirectorStopped(PlayableDirector stoppedDirector) {
            if (stoppedDirector != _director) return;

            ClearPauseTracking();
        }

        private void ClearPauseTracking() {
            _pausedByThisComponent = false;
            _shouldResumeWhenUnblocked = false;
            _wasPlayingBeforePause = false;
        }
    }
}
