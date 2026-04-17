using System;
using UnityEngine;

namespace NullCore
{
    /// <summary>
    /// Attach to any GameObject that has a Renderer.
    /// Reports renderer visibility to both code (via <see cref="Action"/> events)
    /// and the Inspector (via <see cref="StateUnityEvents"/>).
    ///
    /// Unity sends <c>OnBecameVisible</c> / <c>OnBecameInvisible</c> to the Renderer's
    /// own GameObject, so this script must live on that same GameObject.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    [DisallowMultipleComponent]
    public class RenderVisibilityTriggers : MonoBehaviour
    {
        // ── Inspector ────────────────────────────────────────────────────────────

        [SerializeField] bool _debug;

        [Tooltip("On  = became visible by at least one camera.\n" +
                 "Off = became invisible to all cameras.")]
        [SerializeField] StateUnityEvents _visibilityState = new();

        // ── Code-facing events ───────────────────────────────────────────────────

        /// <summary>Raised when the renderer starts being drawn by at least one camera.</summary>
        public event Action BecameVisibleAction;

        /// <summary>Raised when the renderer is no longer drawn by any camera.</summary>
        public event Action BecameInvisibleAction;

        // ── State ────────────────────────────────────────────────────────────────

        /// <summary>Is this renderer currently being drawn by at least one camera?</summary>
        public bool IsCurrentlyVisible { get; private set; }

        // ── Unity messages ───────────────────────────────────────────────────────

        void OnEnable()
        {
            // Re-broadcast the last known state so listeners initialise correctly.
            if (IsCurrentlyVisible)
                BecameVisibleInvoke();
            else
                BecameInvisibleInvoke();
        }

        void OnDisable()
        {
            if (IsCurrentlyVisible)
            {
                IsCurrentlyVisible = false;
                BecameInvisibleInvoke();
            }
        }

        void OnBecameVisible()
        {
            if (IsCurrentlyVisible) return;
            IsCurrentlyVisible = true;
            if (!enabled) return;
            BecameVisibleInvoke();
        }

        void OnBecameInvisible()
        {
            if (!IsCurrentlyVisible) return;
            IsCurrentlyVisible = false;
            if (!enabled) return;
            BecameInvisibleInvoke();
        }

        // ── Invoke helpers ───────────────────────────────────────────────────────

        void BecameVisibleInvoke()
        {
            _visibilityState.Trigger(true);
            BecameVisibleAction?.Invoke();
            if (_debug) Debug.Log($"{name} became visible by some camera.", this);
        }

        void BecameInvisibleInvoke()
        {
            _visibilityState.Trigger(false);
            BecameInvisibleAction?.Invoke();
            if (_debug) Debug.Log($"{name} became invisible to all cameras.", this);
        }
    }
}
