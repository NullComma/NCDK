using UnityEngine;

namespace NCDK
{
    /// <summary>How the quad fills the camera frustum.</summary>
    public enum QuadAspectMode
    {
        /// <summary>Fill the whole frustum. Correct for square images, stretches anything else.</summary>
        Stretch = 0,

        /// <summary>
        /// The frustum height sets the quad height and the aspect sets its width, like
        /// AspectRatioFitter's "height controls width". A wide image covers the screen and its
        /// sides are cropped, but nothing is distorted.
        /// </summary>
        HeightControlsWidth = 1,

        /// <summary>
        /// The frustum width sets the quad width and the aspect sets its height (letterboxed).
        /// Nothing is cropped, but a tall mismatch leaves empty bands.
        /// </summary>
        WidthControlsHeight = 2
    }

    /// <summary>
    /// Scales a quad so it fills the camera frustum at a given distance. With a non-square texture,
    /// pick an <see cref="QuadAspectMode"/> and point <see cref="_aspectSource"/> at it so the image
    /// keeps its proportions instead of being stretched to the screen.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class QuadFrustumScaler : MonoBehaviour
    {
        [SerializeField] Transform _quad;
        [SerializeField] float _distance = 10f;

        [Header("Aspect")]
        [SerializeField] QuadAspectMode _aspectMode = QuadAspectMode.Stretch;

        [Tooltip("Optional. When set, the aspect comes from this texture's width/height, so swapping " +
                 "the image keeps the quad correct.")]
        [SerializeField] Texture _aspectSource;

        [Tooltip("Fallback aspect (width / height) used when no texture is assigned to _aspectSource.")]
        [SerializeField, Min(0.001f)] float _aspect = 1f;

        Camera _camera;
        float _lastFov;
        float _lastAspect;
        float _lastOrthographicSize;
        float _lastDistance;
        bool _lastOrthographic;
        QuadAspectMode _lastMode;
        float _lastTextureAspect;

        /// <summary>Aspect of the source texture when available, otherwise the serialized fallback.</summary>
        public float ResolvedAspect
        {
            get
            {
                if (_aspectSource != null && _aspectSource.height > 0)
                    return (float)_aspectSource.width / _aspectSource.height;

                return Mathf.Max(_aspect, 0.001f);
            }
        }

        void Awake()
        {
            TryGetComponent(out _camera);
            if (_quad == null)
                _quad = transform;
            ScaleToFrustum();
        }

        public void ScaleToFrustum()
        {
            if (_camera == null || _quad == null) return;

            float fov = _camera.fieldOfView;
            float aspect = _camera.aspect;
            float orthographicSize = _camera.orthographicSize;
            bool orthographic = _camera.orthographic;
            float textureAspect = ResolvedAspect;

            if (Mathf.Approximately(fov, _lastFov)
                && Mathf.Approximately(aspect, _lastAspect)
                && Mathf.Approximately(orthographicSize, _lastOrthographicSize)
                && Mathf.Approximately(_distance, _lastDistance)
                && orthographic == _lastOrthographic
                && _aspectMode == _lastMode
                && Mathf.Approximately(textureAspect, _lastTextureAspect))
                return;

            _lastFov = fov;
            _lastAspect = aspect;
            _lastOrthographicSize = orthographicSize;
            _lastDistance = _distance;
            _lastOrthographic = orthographic;
            _lastMode = _aspectMode;
            _lastTextureAspect = textureAspect;

            float frustumHeight = orthographic
                ? 2f * orthographicSize
                : 2f * _distance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * aspect;

            float quadWidth;
            float quadHeight;

            switch (_aspectMode)
            {
                case QuadAspectMode.HeightControlsWidth:
                    quadHeight = frustumHeight;
                    quadWidth = quadHeight * textureAspect;
                    break;

                case QuadAspectMode.WidthControlsHeight:
                    quadWidth = frustumWidth;
                    quadHeight = quadWidth / textureAspect;
                    break;

                default:
                    quadWidth = frustumWidth;
                    quadHeight = frustumHeight;
                    break;
            }

            _quad.localScale = new Vector3(
                quadWidth,
                quadHeight,
                _quad.localScale.z);
        }

        void Update()
        {
            ScaleToFrustum();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            TryGetComponent(out _camera);
            if (_camera != null && Application.isPlaying == false)
                ScaleToFrustum();
        }
#endif
    }
}
