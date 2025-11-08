using UnityEngine;

namespace EnigmaCore
{
    public class SkyboxTrigger : MonoBehaviour
    {
        [SerializeField] Material _targetSkybox;
        
        public void SetSkyboxToTarget()
        {
            SetSkybox(_targetSkybox);
        }

        public void SetSkybox(Material toSet)
        {
            if (toSet == RenderSettings.skybox) return;
            RenderSettings.skybox = toSet;
            DynamicGI.UpdateEnvironment();
            LightProbes.TetrahedralizeAsync();
        }

    }
}