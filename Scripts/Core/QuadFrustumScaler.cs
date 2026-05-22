using UnityEngine;

namespace NCDK
{
    [RequireComponent(typeof(Camera))]
    public class QuadFrustumScaler : MonoBehaviour
    {
        [SerializeField] Transform _quad;
        [SerializeField] float _distance = 10f;

        Camera _camera;
        float _lastFov;
        float _lastAspect;
        float _lastOrthographicSize;
        float _lastDistance;
        bool _lastOrthographic;

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

            if (Mathf.Approximately(fov, _lastFov)
                && Mathf.Approximately(aspect, _lastAspect)
                && Mathf.Approximately(orthographicSize, _lastOrthographicSize)
                && Mathf.Approximately(_distance, _lastDistance)
                && orthographic == _lastOrthographic)
                return;

            _lastFov = fov;
            _lastAspect = aspect;
            _lastOrthographicSize = orthographicSize;
            _lastDistance = _distance;
            _lastOrthographic = orthographic;

            float frustumHeight = orthographic
                ? 2f * orthographicSize
                : 2f * _distance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);

            _quad.localScale = new Vector3(
                frustumHeight * aspect,
                frustumHeight,
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
