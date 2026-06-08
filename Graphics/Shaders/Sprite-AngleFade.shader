Shader "Universal Render Pipeline/2D/Sprite-AngleFade"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)

        [HideInInspector] _Surface("__surface", Float) = 1.0
        [HideInInspector] _Blend("__mode", Float) = 0.0
        [HideInInspector] _BlendOp("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 5.0
        [HideInInspector] _DstBlend("__dst", Float) = 10.0
        [HideInInspector] _SrcBlendAlpha("__srcA", Float) = 1.0
        [HideInInspector] _DstBlendAlpha("__dstA", Float) = 10.0
        [HideInInspector] _ZWrite("__zw", Float) = 0.0
        [HideInInspector] _Cull("__cull", Float) = 0.0
        [HideInInspector] _AlphaToMask("__alphaToMask", Float) = 0.0
        [ToggleUI] _AlphaClip("__clip", Float) = 0.0
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        [ToggleUI] _AngleFadeEnabled("Angle Fade", Float) = 0.0
        _FadeStart("Angle Fade Start", Range(0.0, 1.0)) = 0.0
        _FadeEnd("Angle Fade End", Range(0.0, 1.0)) = 0.5

        [ToggleUI] _CameraFadingEnabled("Camera Fade", Float) = 0.0
        _CameraNearFadeDistance("Camera Near Fade", Float) = 0.0
        _CameraFarFadeDistance("Camera Far Fade", Float) = 2.0
        [HideInInspector] _CameraFadeParams("__camerafadeparams", Vector) = (0,0,0,0)

        [ToggleUI] _SoftParticlesEnabled("Soft Particles", Float) = 0.0
        _SoftParticlesNearFadeDistance("Soft Particles Near Fade", Float) = 0.0
        _SoftParticlesFarFadeDistance("Soft Particles Far Fade", Float) = 1.0
        [HideInInspector] _SoftParticleFadeParams("__softparticlefadeparams", Vector) = (0,0,0,0)

        _QueueOffset("Queue offset", Float) = 0.0
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" "PreviewType" = "Plane"}

        BlendOp[_BlendOp]
        Blend[_SrcBlend][_DstBlend], [_SrcBlendAlpha][_DstBlendAlpha]
        ZWrite[_ZWrite]
        Cull[_Cull]
        AlphaToMask[_AlphaToMask]

        Pass
        {
            HLSLPROGRAM
            #pragma target 2.0
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE
            #pragma shader_feature_local _ANGLEFADE_ON
            #pragma shader_feature_local _NEARFADE_ON
            #pragma shader_feature_local _SOFTPARTICLES_ON
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
                half4 color : COLOR;
                float3 positionWS : TEXCOORD1;
                float4 projectedPosition : TEXCOORD6;
                float3 fadeNormalWS : TEXCOORD4;
                float3 fadeViewDirWS : TEXCOORD5;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            // NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _Cutoff;
                float _FadeStart;
                float _FadeEnd;
                float4 _CameraFadeParams;
                float4 _SoftParticleFadeParams;
            CBUFFER_END

            #if defined(_SOFTPARTICLES_ON)
                TEXTURE2D_X_FLOAT(_CameraDepthTexture);
                float4 _CameraDepthTexture_TexelSize;
                #define sampler_CameraDepthTexture sampler_PointClamp
            #endif

            Varyings UnlitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = CommonUnlitVertex(input);
                o.color = input.color *_Color * unity_SpriteColor;

                float4 ndc = o.positionCS * 0.5f;
                o.projectedPosition.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
                o.projectedPosition.zw = o.positionCS.zw;

                float3 positionWS = TransformObjectToWorld(input.positionOS);
                o.positionWS = positionWS;
                o.fadeNormalWS = TransformObjectToWorldDir(input.normal);
                o.fadeViewDirWS = GetWorldSpaceViewDir(positionWS);
                return o;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                half4 color = CommonUnlitFragment(input, input.color);

                #if defined(_ALPHATEST_ON)
                    clip(color.a - _Cutoff);
                #endif

                #if defined(_ANGLEFADE_ON)
                    half dotNV = saturate(abs(dot(normalize(input.fadeNormalWS), normalize(input.fadeViewDirWS))));
                    color.a *= smoothstep(_FadeStart, _FadeEnd, dotNV);
                #endif

                #if defined(_NEARFADE_ON)
                    half distanceToCamera = length(_WorldSpaceCameraPos.xyz - input.positionWS);
                    color.a *= saturate((distanceToCamera - _CameraFadeParams.x) * _CameraFadeParams.y);
                #endif

                #if defined(_SOFTPARTICLES_ON)
                    float2 screenUV = UnityStereoTransformScreenSpaceTex(input.projectedPosition.xy / input.projectedPosition.w);
                    float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_PointClamp, screenUV).r;
                    float sceneZ = (unity_OrthoParams.w == 0) ? LinearEyeDepth(rawDepth, _ZBufferParams) : LinearDepthToEyeDepth(rawDepth);
                    float thisZ = LinearEyeDepth(input.projectedPosition.z / input.projectedPosition.w, _ZBufferParams);
                    color.a *= saturate(_SoftParticleFadeParams.y * (sceneZ - _SoftParticleFadeParams.x - thisZ));
                #endif

                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/2D/Sprite-Unlit-Default"
    CustomEditor "NCDK.Editor.ParticlesUnlitAngleFadeShaderGUI"
}
