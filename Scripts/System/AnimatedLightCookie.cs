using UnityEngine;
using NCDK.Refs;

namespace NCDK
{
    public class AnimatedLightCookie : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] Light _light;
        [SerializeField, Anywhere] Material _staticMaterial;

        RenderTexture _cookieTexture;

        void Start()
        {
            _cookieTexture = new RenderTexture(128, 128, 0);
            _cookieTexture.Create();
        }

        void OnDestroy()
        {
            if (_cookieTexture != null)
            {
                _cookieTexture.Release();
                Destroy(_cookieTexture);
            }
        }

        void LateUpdate()
        {
            if (!_light.enabled) return;

            Graphics.Blit(null, _cookieTexture, _staticMaterial);
            _light.cookie = _cookieTexture;
        }
    }
}
