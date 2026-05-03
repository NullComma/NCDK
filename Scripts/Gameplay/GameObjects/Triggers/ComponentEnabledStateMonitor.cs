using System;
using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Monitors the <see cref="Behaviour.enabled"/> state of a referenced
    /// <see cref="Behaviour"/> and fires a <see cref="StateUnityEvents"/>
    /// whenever it changes. Works both at runtime and in the Editor.
    /// </summary>
    [AddComponentMenu(StaticStrings.PrefixScripts + "Triggers/Component Enabled State Monitor")]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class ComponentEnabledStateMonitor : MonoBehaviour
    {
        // ── Inspector ────────────────────────────────────────────────────────

        [Tooltip("The Behaviour whose enabled state will be monitored.")]
        [SerializeField] Behaviour _target;

        [Tooltip("On = target became enabled. Off = target became disabled.")]
        [SerializeField] StateUnityEvents _enabledState = new();

        [SerializeField] bool _debug;

        // ── Code-facing events ───────────────────────────────────────────────

        /// <summary>Raised with the new enabled value whenever the target's state changes.</summary>
        public event Action<bool> EnabledStateChanged;

        // ── State ────────────────────────────────────────────────────────────

        /// <summary>The last known enabled state of the target.</summary>
        public bool CurrentState => _lastKnownState;

        bool _lastKnownState;

        // ── Unity messages ───────────────────────────────────────────────────

        void OnEnable() => CacheCurrentState();

        void Reset() => CacheCurrentState();

        void Update()
        {
            // Target destroyed or unassigned — nothing to monitor.
            if (_target == null) return;

            bool current = _target.enabled;
            if (current == _lastKnownState) return;

            _lastKnownState = current;
            _enabledState.Trigger(current);
            EnabledStateChanged?.Invoke(current);

            if (_debug)
                Debug.Log($"{name}: {_target.GetType().Name}.enabled → {current}", this);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        void CacheCurrentState()
        {
            if (_target != null)
                _lastKnownState = _target.enabled;
        }
    }
}
