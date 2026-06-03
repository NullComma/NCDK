#ifndef UNIVERSAL_PARTICLES_UNLIT_ANGLEFADE_INPUT_INCLUDED
#define UNIVERSAL_PARTICLES_UNLIT_ANGLEFADE_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Shaders/Particles/ParticlesInput.hlsl"

CBUFFER_START(UnityPerMaterial)
    float4 _SoftParticleFadeParams;
    float4 _CameraFadeParams;
    float4 _BaseMap_ST;
    float4 _BaseMap_TexelSize;
    half4 _BaseColor;
    half4 _EmissionColor;
    half4 _BaseColorAddSubDiff;
    half _Cutoff;
    half _DistortionStrengthScaled;
    half _DistortionBlend;
    half _Surface;
    float _FadeStart;
    float _FadeEnd;
CBUFFER_END

#ifdef DOTS_INSTANCING_ON
    UNITY_DOTS_INSTANCED_PROP(float, _FadeStart)
    UNITY_DOTS_INSTANCED_PROP(float, _FadeEnd)
    #define _FadeStart_DOTS  UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _FadeStart)
    #define _FadeEnd_DOTS    UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _FadeEnd)
#else
    #define _FadeStart_DOTS  _FadeStart
    #define _FadeEnd_DOTS    _FadeEnd
#endif

#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Particles.hlsl"

#define SOFT_PARTICLE_NEAR_FADE _SoftParticleFadeParams.x
#define SOFT_PARTICLE_INV_FADE_DISTANCE _SoftParticleFadeParams.y

#define CAMERA_NEAR_FADE _CameraFadeParams.x
#define CAMERA_INV_FADE_DISTANCE _CameraFadeParams.y

#define _BumpScale 1.0

half AngleFade(half3 normalWS, half3 viewDirWS)
{
    half dotNV = saturate(abs(dot(normalWS, viewDirWS)));
    return smoothstep(_FadeStart_DOTS, _FadeEnd_DOTS, dotNV);
}

half4 SampleAlbedo(float2 uv, float3 blendUv, half4 color, float4 particleColor, float4 projectedPosition, TEXTURE2D_PARAM(albedoMap, sampler_albedoMap))
{
    half4 albedo = BlendTexture(TEXTURE2D_ARGS(albedoMap, sampler_albedoMap), uv, blendUv) * color;

    half4 colorAddSubDiff = half4(0, 0, 0, 0);
#if defined (_COLORADDSUBDIFF_ON)
    colorAddSubDiff = _BaseColorAddSubDiff;
#endif
    albedo = MixParticleColor(albedo, half4(particleColor), colorAddSubDiff);

    albedo.a = AlphaDiscard(albedo.a, _Cutoff);

    albedo.rgb = AlphaModulateAndPremultiply(albedo.rgb, albedo.a);

#if defined(_SOFTPARTICLES_ON)
    albedo = SOFT_PARTICLE_MUL_ALBEDO(albedo, half(SoftParticles(SOFT_PARTICLE_NEAR_FADE, SOFT_PARTICLE_INV_FADE_DISTANCE, projectedPosition)));
#endif

 #if defined(_FADING_ON)
     ALBEDO_MUL *= CameraFade(CAMERA_NEAR_FADE, CAMERA_INV_FADE_DISTANCE, projectedPosition);
 #endif

    return albedo;
}

half4 SampleAlbedo(TEXTURE2D_PARAM(albedoMap, sampler_albedoMap), ParticleParams params)
{
    half4 albedo = BlendTexture(TEXTURE2D_ARGS(albedoMap, sampler_albedoMap), params.uv, params.blendUv) * params.baseColor;

    #if defined (_COLORADDSUBDIFF_ON)
        half4 colorAddSubDiff = _BaseColorAddSubDiff;
    #else
        half4 colorAddSubDiff = half4(0, 0, 0, 0);
    #endif
    albedo = MixParticleColor(albedo, half4(params.vertexColor), colorAddSubDiff);

    albedo.a = AlphaDiscard(albedo.a, _Cutoff);
    albedo.rgb = AlphaModulateAndPremultiply(albedo.rgb, albedo.a);

    #if defined(_SOFTPARTICLES_ON)
        albedo = SOFT_PARTICLE_MUL_ALBEDO(albedo, half(SoftParticles(SOFT_PARTICLE_NEAR_FADE, SOFT_PARTICLE_INV_FADE_DISTANCE, params)));
    #endif

    #if defined(_FADING_ON)
        ALBEDO_MUL *= CameraFade(CAMERA_NEAR_FADE, CAMERA_INV_FADE_DISTANCE, params.projectedPosition);
    #endif

    return albedo;
}

#endif