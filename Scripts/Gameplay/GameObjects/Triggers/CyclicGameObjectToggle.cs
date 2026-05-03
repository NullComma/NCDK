using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NCDK.Triggers
{
    /// <summary>
    /// Cycles through a list of GameObjects, keeping one active at a time and switching every interval.
    /// Null entries and destroyed objects are skipped safely.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("NCDK/Gameplay/Game Objects/Cyclic GameObject Toggle")]
    public sealed class CyclicGameObjectToggle : MonoBehaviour
    {
        private const float MinimumIntervalSeconds = 0.01f;

        [Header("Target Objects")]
        [Tooltip("GameObjects to cycle through. Only one valid object is kept active at a time.")]
        [SerializeField] private List<GameObject> _targets = new();

        [Header("Timing")]
        [Tooltip("Seconds between each switch to the next valid object in the list.")]
        [Min(MinimumIntervalSeconds)]
        [SerializeField] private float _intervalSeconds = 1f;

        [Tooltip("Index to start from when the component enables for the first time.")]
        [SerializeField] private int _startingIndex;

        private Coroutine _cycleRoutine;
        private int _currentIndex = -1;

        private void Awake()
        {
            ClampConfiguration();
        }

        private void OnEnable()
        {
            ClampConfiguration();
            EnsureCurrentIndex();
            ApplyCurrentState();
            StartCycleRoutine();
        }

        private void OnDisable()
        {
            StopCycleRoutine();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ClampConfiguration();

            if (!Application.isPlaying)
            {
                return;
            }

            EnsureCurrentIndex();
            ApplyCurrentState();
        }
#endif

        private void ClampConfiguration()
        {
            if (_intervalSeconds < MinimumIntervalSeconds)
            {
                _intervalSeconds = MinimumIntervalSeconds;
            }

            _targets ??= new List<GameObject>();

            if (_targets.Count == 0)
            {
                _startingIndex = 0;
                _currentIndex = -1;
                return;
            }

            if (_startingIndex < 0)
            {
                _startingIndex = 0;
            }
            else if (_startingIndex >= _targets.Count)
            {
                _startingIndex = _targets.Count - 1;
            }

            if (_currentIndex >= _targets.Count)
            {
                _currentIndex = -1;
            }
        }

        private void StartCycleRoutine()
        {
            if (_cycleRoutine != null)
            {
                return;
            }

            if (!HasAnyValidTarget())
            {
                return;
            }

            _cycleRoutine = StartCoroutine(CycleRoutine());
        }

        private void StopCycleRoutine()
        {
            if (_cycleRoutine == null)
            {
                return;
            }

            StopCoroutine(_cycleRoutine);
            _cycleRoutine = null;
        }

        private IEnumerator CycleRoutine()
        {
            float elapsed = 0f;

            while (enabled)
            {
                if (!HasAnyValidTarget())
                {
                    _currentIndex = -1;
                    yield return null;
                    continue;
                }

                elapsed += Time.deltaTime;
                if (elapsed < _intervalSeconds)
                {
                    yield return null;
                    continue;
                }

                elapsed = 0f;
                AdvanceToNextTarget();
                yield return null;
            }

            _cycleRoutine = null;
        }

        private void AdvanceToNextTarget()
        {
            if (!TryFindNextValidIndex(GetNextSearchStartIndex(), out int nextIndex))
            {
                _currentIndex = -1;
                ApplyCurrentState();
                return;
            }

            _currentIndex = nextIndex;
            ApplyCurrentState();
        }

        private void EnsureCurrentIndex()
        {
            if (_targets.Count == 0)
            {
                _currentIndex = -1;
                return;
            }

            if (IsValidIndex(_currentIndex))
            {
                GameObject currentTarget = _targets[_currentIndex];
                if (currentTarget != null)
                {
                    return;
                }
            }

            if (!TryFindNextValidIndex(_startingIndex, out int nextIndex))
            {
                _currentIndex = -1;
                return;
            }

            _currentIndex = nextIndex;
        }

        private void ApplyCurrentState()
        {
            if (_targets.Count == 0)
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

                bool shouldBeActive = i == _currentIndex;
                if (target.activeSelf != shouldBeActive)
                {
                    target.SetActive(shouldBeActive);
                }
            }
        }

        private bool HasAnyValidTarget()
        {
            return TryFindNextValidIndex(_currentIndex >= 0 ? _currentIndex : _startingIndex, out _);
        }

        private int GetNextSearchStartIndex()
        {
            if (_targets.Count == 0)
            {
                return -1;
            }

            if (_currentIndex < 0)
            {
                return _startingIndex;
            }

            int nextIndex = _currentIndex + 1;
            return nextIndex >= _targets.Count ? 0 : nextIndex;
        }

        private bool TryFindNextValidIndex(int startIndex, out int nextIndex)
        {
            nextIndex = -1;

            if (_targets.Count == 0)
            {
                return false;
            }

            if (startIndex < 0)
            {
                startIndex = 0;
            }
            else if (startIndex >= _targets.Count)
            {
                startIndex %= _targets.Count;
            }

            for (int offset = 0; offset < _targets.Count; offset++)
            {
                int candidateIndex = (startIndex + offset) % _targets.Count;
                GameObject candidate = _targets[candidateIndex];

                if (candidate == null)
                {
                    continue;
                }

                nextIndex = candidateIndex;
                return true;
            }

            return false;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < _targets.Count;
        }
    }
}
