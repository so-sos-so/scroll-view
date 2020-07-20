// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlphaBendBothSide"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AlphaScale ("AlphaScale", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass{
            Tags { "LightMode"="ForwardBase" }
            // ColorMask RGBA0   RGBA写哪几个，他们就会被写入，0是全都不写入
            ZWrite Off
            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed _AlphaScale;

            struct a2v{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                fixed3 color : COLOR;
                fixed2 uv : TEXCOORD2;
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul( unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                fixed3 worldPos = normalize(i.worldPos);
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed3 albedo = texColor * _Color;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo;
                fixed3 deff = _LightColor0.rgb * albedo * saturate(dot(worldNormal,worldPos));
                return fixed4(ambient + deff, texColor.a * _AlphaScale );
            }

            ENDCG
        }
        
        Pass{
            Tags { "LightMode"="ForwardBase" }
            ZWrite Off
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed _AlphaScale;

            struct a2v{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                fixed3 color : COLOR;
                fixed2 uv : TEXCOORD2;
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul( unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                fixed3 worldPos = normalize(i.worldPos);
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed3 albedo = texColor * _Color;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo;
                fixed3 deff = _LightColor0.rgb * albedo * saturate(dot(worldNormal,worldPos));
                return fixed4(ambient + deff, texColor.a * _AlphaScale );
            }

            ENDCG

        }
    }
    FallBack "Diffuse"
}
