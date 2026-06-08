using UnityEditor;
using UnityEngine;

namespace NCDK.Editor
{
    public class ParticlesUnlitAngleFadeShaderGUI : ShaderGUI
    {
        MaterialProperty angleFadeEnabled;
        MaterialProperty fadeStart;
        MaterialProperty fadeEnd;
        MaterialProperty cameraFadingEnabled;
        MaterialProperty cameraNearFadeDistance;
        MaterialProperty cameraFarFadeDistance;
        MaterialProperty cameraFadeParams;
        MaterialProperty softParticlesEnabled;
        MaterialProperty softParticlesNearFadeDistance;
        MaterialProperty softParticlesFarFadeDistance;
        MaterialProperty softParticleFadeParams;

        MaterialProperty surface;
        MaterialProperty blendMode;
        MaterialProperty cull;
        MaterialProperty alphaClip;
        MaterialProperty cutoff;
        MaterialProperty queueOffset;

        bool angleFadeFoldout = true;
        bool nearFadeFoldout = true;
        bool softParticlesFoldout = true;

        static readonly GUIContent AngleFadeToggle = EditorGUIUtility.TrTextContent("Angle Fade", "Fade alpha based on view angle relative to surface normal.");
        static readonly GUIContent FadeStartLabel = EditorGUIUtility.TrTextContent("Fade Start", "Dot(N,V) value where fade begins (0 = edge-on, 1 = facing).");
        static readonly GUIContent FadeEndLabel = EditorGUIUtility.TrTextContent("Fade End", "Dot(N,V) value where alpha is fully opaque.");

        static readonly GUIContent NearFadeToggle = EditorGUIUtility.TrTextContent("Camera Fade", "Fade alpha based on distance from camera.");
        static readonly GUIContent NearFadeStartLabel = EditorGUIUtility.TrTextContent("Camera Near Fade", "Distance where fade begins (closer = transparent).");
        static readonly GUIContent NearFadeEndLabel = EditorGUIUtility.TrTextContent("Camera Far Fade", "Distance where fade is fully opaque.");

        static readonly GUIContent SoftParticlesToggle = EditorGUIUtility.TrTextContent("Soft Particles", "Fade sprite based on depth difference to scene geometry.");
        static readonly GUIContent SoftParticlesNearLabel = EditorGUIUtility.TrTextContent("Near Fade", "Depth difference where fade begins (0 = on surface).");
        static readonly GUIContent SoftParticlesFarLabel = EditorGUIUtility.TrTextContent("Far Fade", "Depth difference where sprite is fully opaque.");

        static readonly GUIContent SurfaceLabel = EditorGUIUtility.TrTextContent("Surface Type", "Opaque or Transparent surface.");
        static readonly GUIContent BlendLabel = EditorGUIUtility.TrTextContent("Blending Mode", "Controls how the sprite blends with the background.");
        static readonly GUIContent CullLabel = EditorGUIUtility.TrTextContent("Render Face", "Which faces to render.");
        static readonly GUIContent AlphaClipLabel = EditorGUIUtility.TrTextContent("Alpha Clip", "Enable alpha test cutoff.");

        static readonly string[] SurfaceNames = { "Opaque", "Transparent" };
        static readonly string[] BlendNames = { "Alpha", "Premultiply", "Additive", "Multiply" };
        static readonly string[] CullNames = { "Off (Double Sided)", "Front", "Back" };

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            angleFadeEnabled = FindProperty("_AngleFadeEnabled", properties, false);
            fadeStart = FindProperty("_FadeStart", properties);
            fadeEnd = FindProperty("_FadeEnd", properties);
            cameraFadingEnabled = FindProperty("_CameraFadingEnabled", properties, false);
            cameraNearFadeDistance = FindProperty("_CameraNearFadeDistance", properties);
            cameraFarFadeDistance = FindProperty("_CameraFarFadeDistance", properties);
            cameraFadeParams = FindProperty("_CameraFadeParams", properties, false);
            softParticlesEnabled = FindProperty("_SoftParticlesEnabled", properties, false);
            softParticlesNearFadeDistance = FindProperty("_SoftParticlesNearFadeDistance", properties);
            softParticlesFarFadeDistance = FindProperty("_SoftParticlesFarFadeDistance", properties);
            softParticleFadeParams = FindProperty("_SoftParticleFadeParams", properties, false);

            surface = FindProperty("_Surface", properties, false);
            blendMode = FindProperty("_Blend", properties, false);
            cull = FindProperty("_Cull", properties, false);
            alphaClip = FindProperty("_AlphaClip", properties, false);
            cutoff = FindProperty("_Cutoff", properties, false);
            queueOffset = FindProperty("_QueueOffset", properties, false);

            SyncCameraFadeParams(materialEditor);
            SyncSoftParticleFadeParams(materialEditor);

            DrawSurfaceOptions(materialEditor);

            foreach (MaterialProperty prop in properties)
            {
                if ((prop.propertyFlags & UnityEngine.Rendering.ShaderPropertyFlags.HideInInspector) != 0)
                    continue;
                if (prop.propertyFlags.HasFlag(UnityEngine.Rendering.ShaderPropertyFlags.PerRendererData))
                    continue;
                if (prop == angleFadeEnabled || prop == fadeStart || prop == fadeEnd ||
                    prop == cameraFadingEnabled || prop == cameraNearFadeDistance || prop == cameraFarFadeDistance ||
                    prop == cameraFadeParams ||
                    prop == softParticlesEnabled || prop == softParticlesNearFadeDistance || prop == softParticlesFarFadeDistance ||
                    prop == softParticleFadeParams ||
                    prop == surface || prop == blendMode || prop == cull || prop == alphaClip || prop == cutoff ||
                    prop == queueOffset)
                    continue;
                materialEditor.ShaderProperty(prop, prop.displayName);
            }

            DrawAngleFadeSection(materialEditor);
            DrawNearFadeSection(materialEditor);
            DrawSoftParticlesSection(materialEditor);
        }

        void DrawSurfaceOptions(MaterialEditor materialEditor)
        {
            EditorGUILayout.Space(5);

            EditorGUI.BeginChangeCheck();
            int surfaceValue = EditorGUILayout.Popup(SurfaceLabel, (int)surface.floatValue, SurfaceNames);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object target in materialEditor.targets)
                {
                    Material m = (Material)target;
                    m.SetFloat("_Surface", surfaceValue);
                    if (surfaceValue == 0)
                    {
                        m.SetOverrideTag("RenderType", "Opaque");
                        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        m.SetInt("_SrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
                        m.SetInt("_DstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.Zero);
                        m.SetInt("_ZWrite", 1);
                        m.SetInt("_AlphaToMask", 0);
                        m.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry + (int)m.GetFloat("_QueueOffset");
                    }
                    else
                    {
                        m.SetOverrideTag("RenderType", "Transparent");
                        m.SetInt("_ZWrite", 0);
                        m.SetInt("_AlphaToMask", 0);
                        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        ApplyBlendMode(m, (int)m.GetFloat("_Blend"));
                        m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + (int)m.GetFloat("_QueueOffset");
                    }
                }
            }

            EditorGUI.BeginDisabledGroup(surfaceValue == 0);
            EditorGUI.BeginChangeCheck();
            int blendValue = EditorGUILayout.Popup(BlendLabel, (int)blendMode.floatValue, BlendNames);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object target in materialEditor.targets)
                {
                    Material m = (Material)target;
                    m.SetFloat("_Blend", blendValue);
                    ApplyBlendMode(m, blendValue);
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();
            int cullValue = EditorGUILayout.Popup(CullLabel, (int)cull.floatValue, CullNames);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object target in materialEditor.targets)
                {
                    Material m = (Material)target;
                    m.SetFloat("_Cull", cullValue);
                }
            }

            EditorGUI.BeginChangeCheck();
            bool clip = EditorGUILayout.Toggle(AlphaClipLabel, alphaClip.floatValue > 0.5f);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object target in materialEditor.targets)
                {
                    Material m = (Material)target;
                    m.SetFloat("_AlphaClip", clip ? 1f : 0f);
                    if (clip)
                        m.EnableKeyword("_ALPHATEST_ON");
                    else
                        m.DisableKeyword("_ALPHATEST_ON");
                }
            }

            if (alphaClip.floatValue > 0.5f)
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(cutoff, "Threshold");
                EditorGUI.indentLevel--;
            }

            EditorGUI.BeginChangeCheck();
            materialEditor.ShaderProperty(queueOffset, "Queue Offset");
            if (EditorGUI.EndChangeCheck() && queueOffset != null)
            {
                foreach (Object target in materialEditor.targets)
                {
                    Material m = (Material)target;
                    m.renderQueue = (m.GetFloat("_Surface") > 0.5f)
                        ? (int)UnityEngine.Rendering.RenderQueue.Transparent + (int)m.GetFloat("_QueueOffset")
                        : (int)UnityEngine.Rendering.RenderQueue.Geometry + (int)m.GetFloat("_QueueOffset");
                }
            }
        }

        static void ApplyBlendMode(Material m, int blend)
        {
            switch (blend)
            {
                case 0: // Alpha
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    m.SetInt("_SrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
                    m.SetInt("_DstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
                case 1: // Premultiply
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    m.SetInt("_SrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
                    m.SetInt("_DstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
                case 2: // Additive
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    m.SetInt("_SrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    m.SetInt("_DstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case 3: // Multiply
                    m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    m.SetInt("_SrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.DstAlpha);
                    m.SetInt("_DstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
            }
        }

        void SyncCameraFadeParams(MaterialEditor materialEditor)
        {
            if (cameraFadeParams == null) return;
            foreach (Object target in materialEditor.targets)
            {
                Material m = (Material)target;
                float near = m.GetFloat("_CameraNearFadeDistance");
                float far = m.GetFloat("_CameraFarFadeDistance");
                float invDist = (far - near) > 0.001f ? 1.0f / (far - near) : 0.0f;
                Vector4 expected = new Vector4(near, invDist, 0, 0);
                if (m.GetVector("_CameraFadeParams") != expected)
                    m.SetVector("_CameraFadeParams", expected);
            }
        }

        void SyncSoftParticleFadeParams(MaterialEditor materialEditor)
        {
            if (softParticleFadeParams == null) return;
            foreach (Object target in materialEditor.targets)
            {
                Material m = (Material)target;
                float near = m.GetFloat("_SoftParticlesNearFadeDistance");
                float far = m.GetFloat("_SoftParticlesFarFadeDistance");
                float invDist = (far - near) > 0.001f ? 1.0f / (far - near) : 0.0f;
                Vector4 expected = new Vector4(near, invDist, 0, 0);
                if (m.GetVector("_SoftParticleFadeParams") != expected)
                    m.SetVector("_SoftParticleFadeParams", expected);
            }
        }

        void DrawAngleFadeSection(MaterialEditor materialEditor)
        {
            EditorGUILayout.Space(5);
            angleFadeFoldout = EditorGUILayout.Foldout(angleFadeFoldout, "Angle Fade", true);
            if (angleFadeFoldout)
            {
                EditorGUI.indentLevel++;

                bool enabled = false;
                if (angleFadeEnabled != null)
                {
                    EditorGUI.BeginChangeCheck();
                    enabled = EditorGUILayout.Toggle(AngleFadeToggle, angleFadeEnabled.floatValue > 0.5f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        angleFadeEnabled.floatValue = enabled ? 1f : 0f;
                        foreach (Object target in materialEditor.targets)
                        {
                            Material m = (Material)target;
                            if (enabled)
                                m.EnableKeyword("_ANGLEFADE_ON");
                            else
                                m.DisableKeyword("_ANGLEFADE_ON");
                        }
                    }
                }
                else
                {
                    Object[] targets = materialEditor.targets;
                    Material mat = targets.Length > 0 ? targets[0] as Material : null;
                    if (mat != null)
                    {
                        enabled = mat.IsKeywordEnabled("_ANGLEFADE_ON");
                        EditorGUI.BeginChangeCheck();
                        enabled = EditorGUILayout.Toggle(AngleFadeToggle, enabled);
                        if (EditorGUI.EndChangeCheck())
                        {
                            foreach (Object target in materialEditor.targets)
                            {
                                Material m = (Material)target;
                                if (enabled)
                                    m.EnableKeyword("_ANGLEFADE_ON");
                                else
                                    m.DisableKeyword("_ANGLEFADE_ON");
                            }
                        }
                    }
                }

                EditorGUI.BeginDisabledGroup(!enabled);
                materialEditor.ShaderProperty(fadeStart, FadeStartLabel);
                materialEditor.ShaderProperty(fadeEnd, FadeEndLabel);
                EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel--;
            }
        }

        void DrawNearFadeSection(MaterialEditor materialEditor)
        {
            EditorGUILayout.Space(5);
            nearFadeFoldout = EditorGUILayout.Foldout(nearFadeFoldout, "Camera Fade", true);
            if (nearFadeFoldout)
            {
                EditorGUI.indentLevel++;

                bool enabled = false;
                if (cameraFadingEnabled != null)
                {
                    EditorGUI.BeginChangeCheck();
                    enabled = EditorGUILayout.Toggle(NearFadeToggle, cameraFadingEnabled.floatValue > 0.5f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        cameraFadingEnabled.floatValue = enabled ? 1f : 0f;
                        foreach (Object target in materialEditor.targets)
                        {
                            Material m = (Material)target;
                            if (enabled)
                                m.EnableKeyword("_NEARFADE_ON");
                            else
                                m.DisableKeyword("_NEARFADE_ON");
                        }
                    }
                }
                else
                {
                    Object[] targets = materialEditor.targets;
                    Material mat = targets.Length > 0 ? targets[0] as Material : null;
                    if (mat != null)
                    {
                        enabled = mat.IsKeywordEnabled("_NEARFADE_ON");
                        EditorGUI.BeginChangeCheck();
                        enabled = EditorGUILayout.Toggle(NearFadeToggle, enabled);
                        if (EditorGUI.EndChangeCheck())
                        {
                            foreach (Object target in materialEditor.targets)
                            {
                                Material m = (Material)target;
                                if (enabled)
                                    m.EnableKeyword("_NEARFADE_ON");
                                else
                                    m.DisableKeyword("_NEARFADE_ON");
                            }
                        }
                    }
                }

                EditorGUI.BeginDisabledGroup(!enabled);
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(cameraNearFadeDistance, NearFadeStartLabel);
                materialEditor.ShaderProperty(cameraFarFadeDistance, NearFadeEndLabel);
                if (EditorGUI.EndChangeCheck() && cameraFadeParams != null)
                {
                    foreach (Object target in materialEditor.targets)
                    {
                        Material m = (Material)target;
                        float near = m.GetFloat("_CameraNearFadeDistance");
                        float far = m.GetFloat("_CameraFarFadeDistance");
                        float invDist = (far - near) > 0.001f ? 1.0f / (far - near) : 0.0f;
                        m.SetVector("_CameraFadeParams", new Vector4(near, invDist, 0, 0));
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel--;
            }
        }

        void DrawSoftParticlesSection(MaterialEditor materialEditor)
        {
            EditorGUILayout.Space(5);
            softParticlesFoldout = EditorGUILayout.Foldout(softParticlesFoldout, "Soft Particles", true);
            if (softParticlesFoldout)
            {
                EditorGUI.indentLevel++;

                bool enabled = false;
                if (softParticlesEnabled != null)
                {
                    EditorGUI.BeginChangeCheck();
                    enabled = EditorGUILayout.Toggle(SoftParticlesToggle, softParticlesEnabled.floatValue > 0.5f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        softParticlesEnabled.floatValue = enabled ? 1f : 0f;
                        foreach (Object target in materialEditor.targets)
                        {
                            Material m = (Material)target;
                            if (enabled)
                                m.EnableKeyword("_SOFTPARTICLES_ON");
                            else
                                m.DisableKeyword("_SOFTPARTICLES_ON");
                        }
                    }
                }
                else
                {
                    Object[] targets = materialEditor.targets;
                    Material mat = targets.Length > 0 ? targets[0] as Material : null;
                    if (mat != null)
                    {
                        enabled = mat.IsKeywordEnabled("_SOFTPARTICLES_ON");
                        EditorGUI.BeginChangeCheck();
                        enabled = EditorGUILayout.Toggle(SoftParticlesToggle, enabled);
                        if (EditorGUI.EndChangeCheck())
                        {
                            foreach (Object target in materialEditor.targets)
                            {
                                Material m = (Material)target;
                                if (enabled)
                                    m.EnableKeyword("_SOFTPARTICLES_ON");
                                else
                                    m.DisableKeyword("_SOFTPARTICLES_ON");
                            }
                        }
                    }
                }

                EditorGUI.BeginDisabledGroup(!enabled);
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(softParticlesNearFadeDistance, SoftParticlesNearLabel);
                materialEditor.ShaderProperty(softParticlesFarFadeDistance, SoftParticlesFarLabel);
                if (EditorGUI.EndChangeCheck() && softParticleFadeParams != null)
                {
                    foreach (Object target in materialEditor.targets)
                    {
                        Material m = (Material)target;
                        float near = m.GetFloat("_SoftParticlesNearFadeDistance");
                        float far = m.GetFloat("_SoftParticlesFarFadeDistance");
                        float invDist = (far - near) > 0.001f ? 1.0f / (far - near) : 0.0f;
                        m.SetVector("_SoftParticleFadeParams", new Vector4(near, invDist, 0, 0));
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel--;
            }
        }
    }
}
