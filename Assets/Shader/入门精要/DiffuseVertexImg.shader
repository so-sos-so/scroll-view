// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DiffuseVertexImg"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1,1,1,1)
        _Color ("Color", Color) = (1,1,1,1)
        _RampTex ("RampTex",2D) = "white" {}
        _Gloss ("Gloss", Range(8.0, 256)) = 20
        _Specular ("Specular", Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass{
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            fixed4 _Diffuse;
            fixed4 _Color;
            sampler2D _RampTex;
            float4 _RampTex_ST;
            float _Gloss;
            fixed4 _Specular;

            struct a2v{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                fixed3 worldNormal : TEXCOORD1;
                fixed2 uv : TEXCOORD2;
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _RampTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                fixed3 lightDir = UnityWorldSpaceLightDir(i.worldPos);
                fixed diff = dot(lightDir, i.worldNormal) * 0.5 + 0.5;
                fixed4 ramp = tex2D(_RampTex, i.uv);
                fixed3 diffColor = _LightColor0.rgb * diff * ramp.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * ramp.rgb;
                fixed3 result = ambient + diffColor;
                return fixed4(result , 1);
            }

            ENDCG

        }
    }
    FallBack "Diffuse"
}
