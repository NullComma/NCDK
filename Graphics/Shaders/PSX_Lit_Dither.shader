Shader "EnigmaCore/PSX_Lit_Dither_ForwardPlus_Final"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor]   _BaseColor("Base Color", Color) = (1, 1, 1, 1)

        [Header(Emission)]
        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0,1)
        [NoScaleOffset] _EmissionMap("Emission Map", 2D) = "white" {}

        [Header(Dither Fade)]
        _FadeStart("Fade Start Distance", Float) = 2.0
        _FadeWidth("Fade Width", Float) = 3.0
        
        // Necessário para Batching
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry"
            "UniversalMaterialType" = "SimpleLit"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On

            HLSLPROGRAM
            // Forward+ EXIGE Shader Model 4.5 (Compute capability)
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            // --- KEYWORDS CRUCIAIS ---
            // Esta linha define as variantes. Sem _FORWARD_PLUS aqui, o Clustering.hlsl é ignorado.
            #pragma multi_compile _ _FORWARD_PLUS
            
            // Fallback para modo normal
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            // O Clustering.hlsl já vem dentro do Lighting.hlsl -> RealtimeLights.hlsl,
            // desde que _FORWARD_PLUS esteja definido acima.

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD3;
                float fogFactor : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                half4 _EmissionColor;
                float _FadeStart;
                float _FadeWidth;
            CBUFFER_END

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                return output;
            }

            half GetDitherThreshold(float2 screenPos)
            {
                const half dither[16] = {
                    0.0588, 0.5294, 0.1765, 0.6471,
                    0.7647, 0.2941, 0.8824, 0.4118,
                    0.2353, 0.7059, 0.1176, 0.5882,
                    0.9412, 0.4706, 0.8235, 0.3529
                };
                uint x = (uint)screenPos.x % 4;
                uint y = (uint)screenPos.y % 4;
                return dither[x + y * 4];
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                // 1. DITHER FADE
                float dist = distance(_WorldSpaceCameraPos, input.positionWS);
                float fadeFactor = saturate((dist - _FadeStart) / max(0.001, _FadeWidth));
                clip(fadeFactor - GetDitherThreshold(input.positionCS.xy));

                // 2. PREPARAÇÃO DE DADOS
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                half3 normalWS = normalize(input.normalWS);
                
                // --- CONSTRUÇÃO DO InputData (Essencial para a Macro funcionar) ---
                InputData inputData = (InputData)0; // Zera tudo primeiro
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                // Esta linha é o segredo: O Forward+ precisa das UVs de tela normalizadas para achar o tile
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = half4(1, 1, 1, 1);
                
                // 3. ILUMINAÇÃO
                // Ambiente
                half3 lighting = SampleSH(normalWS); 
                
                // Principal
                Light mainLight = GetMainLight(inputData.shadowCoord);
                lighting += LightingLambert(mainLight.color, mainLight.direction, normalWS) * mainLight.shadowAttenuation;

                // --- LUZES ADICIONAIS (COM MACRO) ---
                // GetAdditionalLightsCount() retorna 0 no Forward+, mas a gente passa ele mesmo assim
                // porque a Macro ignora esse valor se estiver em Forward+ e usa o Cluster Loop interno.
                uint pixelLightCount = GetAdditionalLightsCount();
                
                // Esta macro expande para:
                // - Forward+: ClusterInit + while(ClusterNext) ...
                // - Forward: for (i < count) ...
                // Ela exige que a variável 'inputData' e 'pixelLightCount' existam.
                LIGHT_LOOP_BEGIN(pixelLightCount)
                    // Dentro da macro, a variável 'lightIndex' é criada automaticamente
                    Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
                    
                    lighting += LightingLambert(light.color, light.direction, normalWS) * (light.distanceAttenuation * light.shadowAttenuation);
                LIGHT_LOOP_END

                half3 finalColor = albedo.rgb * lighting;

                // 4. EMISSION & FOG
                half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb;
                finalColor += emission;
                finalColor = MixFog(finalColor, input.fogFactor);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On
            ColorMask 0
            Cull Back
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
}