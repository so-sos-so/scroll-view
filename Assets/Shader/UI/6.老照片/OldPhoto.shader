Shader "Hidden/OldPhoto"
{
Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        Level ("程度", Range(0,1)) = 0.5
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
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;

            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;
            fixed Level;

            fixed4 SampleSpriteTexture (float2 uv)
            {

                fixed4 color = tex2D (_MainTex, uv);
                half R= 0.393 * color.r + 0.769 * color.g + 0.189 * color.b;
                half G = 0.349 * color.r + 0.686 * color.g + 0.168 * color.b;
                half B = 0.272 * color.r + 0.534 * color.g + 0.131 * color.b;
                fixed4 targetColor = fixed4(R,G,B,1);
                color = lerp(color, targetColor, Level);
                
                
#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if (_AlphaSplitEnabled)
                    color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

                return color;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}

