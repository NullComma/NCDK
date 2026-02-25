Shader "EnigmaCore/2D/Sprite Grayscale"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1) // Se nunca usar Tint, pode apagar esta linha
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                fixed4 color    : COLOR;     // Mudado para fixed4 (mais barato)
                half2 texcoord  : TEXCOORD0; // Mudado de float2 para half2
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0; // Mudado de float2 para half2
            };

            fixed4 _Color; // Se nunca usar Tint, pode apagar esta linha

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                
                // Se nunca usar Tint, mude para: OUT.color = IN.color;
                OUT.color = IN.color * _Color; 
                
                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Produto escalar (dot) é uma instrução nativa super rápida de hardware
                fixed gray = dot(c.rgb, fixed3(0.299, 0.587, 0.114));
                
                // Usando swizzling (.xxx) para montar a cor de forma ligeiramente mais rápida
                return fixed4(gray.xxx, c.a);
            }
            ENDCG
        }
    }
}