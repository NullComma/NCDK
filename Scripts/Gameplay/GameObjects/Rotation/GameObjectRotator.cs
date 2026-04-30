using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Rotates the GameObject. Handles visibility checks for standard Renderers 
    /// and relies on OnEnable/OnDisable for UI Canvas elements.
    /// </summary>
    public class GameObjectRotator : MonoBehaviourUpdateExecutionLoopTime
    {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] Vector3 _rotateDirectionAndSpeed;
        [SerializeField] bool _onlyUpdateWhenVisible = true;
        [SerializeField] bool _ignoreTimescale;

        [System.NonSerialized] bool _isVisible;
        [System.NonSerialized] bool _isUIElement;
        [System.NonSerialized] Transform _transform;

        #endregion <<---------- Properties and Fields ---------->>

        void Awake()
        {
            // Caching the transform is slightly faster in heavy loops
            _transform = transform;

            // We only need to know if it's UI to handle the visibility logic
            _isUIElement = GetComponent<RectTransform>() != null;
        }

        void OnEnable()
        {
            if (_isUIElement) _isVisible = true;
        }

        void OnDisable()
        {
            if (_isUIElement) _isVisible = false;
        }

        void OnBecameVisible()
        {
            if (!_isUIElement) _isVisible = true;
        }

        void OnBecameInvisible()
        {
            if (!_isUIElement) _isVisible = false;
        }

        protected override void Execute(float deltaTime)
        {
            if (_onlyUpdateWhenVisible && !_isVisible) return;

            // Uses the unscaled time if requested, otherwise uses the deltaTime 
            // properly injected by your base class (Update, FixedUpdate, or LateUpdate).
            float dt = _ignoreTimescale ? Time.unscaledDeltaTime : deltaTime;

            _transform.Rotate(_rotateDirectionAndSpeed * dt);
        }
    }
}