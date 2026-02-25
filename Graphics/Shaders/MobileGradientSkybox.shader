Shader "Custom/MobileScreenGradientSkyboxURP"
{
    Properties
    {
        _ColorTop ("Top Color", Color) = (0.2, 0.5, 0.8, 1.0)
        _ColorBottom ("Bottom Color", Color) = (0.8, 0.9, 1.0, 1.0)
        _Rotation ("Rotation", Range(0, 360)) = 0
        _Scale ("Gradient Scale", Float) = 1.0
        _Offset ("Gradient Offset", Float) = 0.0
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Background" 
            "RenderType"="Background" 
            "RenderPipeline"="UniversalPipeline"
            "PreviewType"="Skybox" 
        }
        
        Cull Off 
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _ColorTop;
                half4 _ColorBottom;
                float _Rotation;
                float _Scale;
                float _Offset;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                // Transform to clip space using URP standard functions
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.screenPos = ComputeScreenPos(o.positionHCS);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Normalize screen UVs (0 to 1)
                float2 uv = i.screenPos.xy / i.screenPos.w;
                
                // Aspect Ratio correction to ensure rotation doesn't stretch/squash the gradient
                float aspect = _ScreenParams.x / _ScreenParams.y;
                
                // Center the UVs around (0,0) before rotating
                uv = uv - 0.5;
                
                // Apply aspect correction to the X axis
                uv.x *= aspect;
                
                // Convert rotation to radians
                float angle = _Rotation * (PI / 180.0);
                float sinA, cosA;
                sincos(angle, sinA, cosA);
                
                // Rotate the UVs
                float2 rotatedUV;
                rotatedUV.x = uv.x * cosA - uv.y * sinA;
                rotatedUV.y = uv.x * sinA + uv.y * cosA;
                
                // We base the gradient on the rotated Y axis
                // Re-add the 0.5 offset so it's centered, apply scale and user offset
                float t = (rotatedUV.y / _Scale) + 0.5 + _Offset;
                
                // Clamp between 0 and 1 so colors don't exceed the chosen properties
                t = saturate(t);

                // Blend between bottom and top color based on the rotated vertical value
                return lerp(_ColorBottom, _ColorTop, t);
            }
            ENDHLSL
        }
    }
}
