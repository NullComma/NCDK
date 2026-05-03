using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCDK;

namespace NCDK.Triggers
{
    /// <summary>
    /// Repeatedly toggles a list of target GameObjects on and off every interval while this component is enabled.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("NCDK/Gameplay/Game Objects/Simple GameObject Toggle")]
    public sealed class SimpleGameObjectToggle : MonoBehaviour
    {
        private const float MinimumIntervalSeconds = 0.01f;

        [Header("Target Objects")]
        [Tooltip("GameObjects to toggle together between active and inactive states.")]
        [SerializeField] private List<GameObject> _targets = new();

        [Header("Timing")]
        [Tooltip("Seconds between each toggle.")]
        [Min(MinimumIntervalSeconds)]
        [SerializeField] private float _intervalSeconds = 1f;

        [Tooltip("Initial state applied when the component becomes enabled.")]
        [SerializeField] private bool _startActive = true;

        private Coroutine _toggleRoutine;
        private WaitForSeconds _cachedWait;
        private bool _isActiveState;

        private void Awake()
        {
            ClampConfiguration();
            RebuildWaitCache();
            _isActiveState = _startActive;
            ApplyState(_isActiveState);
        }

        private void OnEnable()
        {
            ClampConfiguration();
            RebuildWaitCache();
            _isActiveState = _startActive;
            ApplyState(_isActiveState);
            StartToggleRoutine();
        }

        private void OnDisable()
        {
            StopToggleRoutine();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ClampConfiguration();

            if (!Application.isPlaying)
            {
                return;
            }

            RebuildWaitCache();
        }
#endif

        [Button]
        public void PopulateTargetsFromChildren()
        {
            _targets ??= new List<GameObject>();
            _targets.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child == null)
                {
                    continue;
                }

                _targets.Add(child.gameObject);
            }
        }

        private void StartToggleRoutine()
        {
            if (_toggleRoutine != null || !HasAnyValidTarget())
            {
                return;
            }

            _toggleRoutine = StartCoroutine(ToggleRoutine());
        }

        private void StopToggleRoutine()
        {
            if (_toggleRoutine == null)
            {
                return;
            }

            StopCoroutine(_toggleRoutine);
            _toggleRoutine = null;
        }

        private IEnumerator ToggleRoutine()
        {
            while (enabled)
            {
                yield return _cachedWait;

                if (!HasAnyValidTarget())
                {
                    break;
                }

                _isActiveState = !_isActiveState;
                ApplyState(_isActiveState);
            }

            _toggleRoutine = null;
        }

        private void ApplyState(bool activeState)
        {
            if (_targets == null || _targets.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _targets.Count; i++)
            {
                GameObject target = _targets[i];
                if (target == null)
                {
                    continue;
                }

                if (target.activeSelf != activeState)
                {
                    target.SetActive(activeState);
                }
            }
        }

        private bool HasAnyValidTarget()
        {
            if (_targets == null)
            {
                return false;
            }

            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i] != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void ClampConfiguration()
        {
            if (_intervalSeconds < MinimumIntervalSeconds)
            {
                _intervalSeconds = MinimumIntervalSeconds;
            }
        }

        private void RebuildWaitCache()
        {
            _cachedWait = new WaitForSeconds(_intervalSeconds);
        }
    }
}
