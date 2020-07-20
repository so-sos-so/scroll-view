Shader "Hidden/EdgeDetection"
{
Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        Blurry ("程度", Range(0,0.1)) = 0.01
        BgColor ("背景颜色", Color) = (1,1,1,1)
        EdgeColor ("边缘颜色", Color) = (0,0,0,1)
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

            struct appdata_t
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
            fixed4 BgColor;
            fixed4 EdgeColor;

            v2f vert(appdata_t IN)
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
            fixed Blurry;

            fixed lightness(fixed4 color)
            {
                return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b; 
            }

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);
                
                // 左
                half gx = lightness(tex2D (_MainTex, fixed2(uv.x-Blurry,uv.y))) * -2;
                // 左上
                gx += lightness(tex2D(_MainTex, fixed2(uv.x - Blurry, uv.y + Blurry))) * -1;
                // 左下
                gx += lightness(tex2D(_MainTex, fixed2(uv.x - Blurry, uv.y - Blurry))) * -1;
                // 右
                gx += lightness(tex2D(_MainTex, fixed2(uv.x+Blurry,uv.y))) * 2;
                // 右上
                gx += lightness(tex2D(_MainTex, fixed2(uv.x + Blurry, uv.y + Blurry))) * 1;
                // 右下
                gx += lightness(tex2D(_MainTex, fixed2(uv.x + Blurry, uv.y - Blurry))) * 1;
                // 上
                gx += lightness(tex2D(_MainTex, fixed2(uv.x, uv.y - Blurry))) * 0;
                // 下
                gx += lightness(tex2D(_MainTex, fixed2(uv.x, uv.y + Blurry))) * 0;


                // 左
                half gy = lightness(tex2D (_MainTex, fixed2(uv.x-Blurry,uv.y))) * 0;
                // 左上
                gy += lightness(tex2D(_MainTex, fixed2(uv.x - Blurry, uv.y + Blurry))) * -1;
                // 左下
                gy += lightness(tex2D(_MainTex, fixed2(uv.x - Blurry, uv.y - Blurry))) * 1;
                // 右
                gy += lightness(tex2D (_MainTex, fixed2(uv.x+Blurry,uv.y))) * 0;
                // 右上
                gy += lightness(tex2D(_MainTex, fixed2(uv.x + Blurry, uv.y + Blurry))) * -1;
                // 右下
                gy += lightness(tex2D(_MainTex, fixed2(uv.x + Blurry, uv.y - Blurry))) * 1;
                // 上
                gy += lightness(tex2D(_MainTex, fixed2(uv.x, uv.y - Blurry))) * -2;
                // 下
                gy += lightness(tex2D(_MainTex, fixed2(uv.x, uv.y + Blurry))) * 2;

                half edge = abs(gx) + abs(gy);

                return lerp(color * BgColor, EdgeColor, edge);
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

