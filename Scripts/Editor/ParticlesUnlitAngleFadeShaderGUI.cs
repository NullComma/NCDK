using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Rendering.Universal.ShaderGUI;

namespace NCDK.Editor
{
    public class ParticlesUnlitAngleFadeShaderGUI : BaseShaderGUI
    {
        private BakedLitGUI.BakedLitProperties shadingModelProperties;
        private ParticleGUI.ParticleProperties particleProps;
        private MaterialProperty angleFadeEnabled;
        private MaterialProperty fadeStart;
        private MaterialProperty fadeEnd;
        private List<ParticleSystemRenderer> renderersUsingThisMaterial = new List<ParticleSystemRenderer>();

        private static readonly GUIContent AngleFadeToggle = EditorGUIUtility.TrTextContent("Angle Fade", "Fade alpha based on view angle relative to surface normal.");
        private static readonly GUIContent FadeStartLabel = EditorGUIUtility.TrTextContent("Fade Start", "Dot(N,V) value where fade begins (0 = edge-on, 1 = facing).");
        private static readonly GUIContent FadeEndLabel = EditorGUIUtility.TrTextContent("Fade End", "Dot(N,V) value where alpha is fully opaque.");

        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            shadingModelProperties = new BakedLitGUI.BakedLitProperties(properties);
            particleProps = new ParticleGUI.ParticleProperties(properties);
            angleFadeEnabled = FindProperty("_AngleFadeEnabled", properties, false);
            fadeStart = FindProperty("_FadeStart", properties);
            fadeEnd = FindProperty("_FadeEnd", properties);
        }

        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, null, ParticleGUI.SetMaterialKeywords);

            bool angleFade_on = angleFadeEnabled != null ? angleFadeEnabled.floatValue > 0.5f : material.IsKeywordEnabled("_ANGLEFADE_ON");
            if (angleFade_on)
                material.EnableKeyword("_ANGLEFADE_ON");
            else
                material.DisableKeyword("_ANGLEFADE_ON");
        }

        public override void DrawSurfaceOptions(Material material)
        {
            base.DrawSurfaceOptions(material);
            DoPopup(ParticleGUI.Styles.colorMode, particleProps.colorMode, System.Enum.GetNames(typeof(ParticleGUI.ColorMode)));
        }

        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            BakedLitGUI.Inputs(shadingModelProperties, materialEditor);
            DrawEmissionProperties(material, true);
        }

        public override void DrawAdvancedOptions(Material material)
        {
            materialEditor.ShaderProperty(particleProps.flipbookMode, ParticleGUI.Styles.flipbookMode);
            ParticleGUI.FadingOptions(material, materialEditor, particleProps);
            ParticleGUI.DoVertexStreamsArea(material, renderersUsingThisMaterial);

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();
            bool enabled;
            if (angleFadeEnabled != null)
                enabled = EditorGUILayout.Toggle(AngleFadeToggle, angleFadeEnabled.floatValue > 0.5f);
            else
                enabled = EditorGUILayout.Toggle(AngleFadeToggle, material.IsKeywordEnabled("_ANGLEFADE_ON"));

            if (EditorGUI.EndChangeCheck())
            {
                if (angleFadeEnabled != null)
                    angleFadeEnabled.floatValue = enabled ? 1f : 0f;
                if (enabled)
                    material.EnableKeyword("_ANGLEFADE_ON");
                else
                    material.DisableKeyword("_ANGLEFADE_ON");
            }

            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!enabled);
            materialEditor.ShaderProperty(fadeStart, FadeStartLabel);
            materialEditor.ShaderProperty(fadeEnd, FadeEndLabel);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            DrawQueueOffsetField();
        }

        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            CacheRenderersUsingThisMaterial(material);
            base.OnOpenGUI(material, materialEditor);
        }

        void CacheRenderersUsingThisMaterial(Material material)
        {
            renderersUsingThisMaterial.Clear();
            ParticleSystemRenderer[] renderers = UnityEngine.Object.FindObjectsByType<ParticleSystemRenderer>(FindObjectsSortMode.InstanceID);
            foreach (ParticleSystemRenderer renderer in renderers)
            {
                if (renderer.sharedMaterial == material)
                    renderersUsingThisMaterial.Add(renderer);
            }
        }
    }
}