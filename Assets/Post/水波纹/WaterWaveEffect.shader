// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/WaterWaveEffect" {
    Properties {
        _MainTex("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Pass{
            Tags { "RenderType"="Opaque" }
            
            CGPROGRAM
            #include "UnityCG.cginc"
            
            #pragma vertex vert
            #pragma fragment frag
    
            sampler2D _MainTex;
            float _distanceFactor;
    
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };
            
            v2f vert(appdata_img v)
            {
                v2f f;
                f.pos = UnityObjectToClipPos(v.vertex);
                f.uv = v.texcoord;
                return f;
            }
            
            fixed4 frag(v2f f) : SV_Target
            {
                half2 uv = half2(0.5,0.5) - f.uv;
                uv = normalize(uv);
                f.uv = uv * _distanceFactor + f.uv;
                fixed4 color = tex2D(_MainTex, f.uv);
                return color;
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"
}