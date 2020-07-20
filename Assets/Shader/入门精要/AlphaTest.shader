// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/AlphaTest"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white"{}
        _CutOff ("CutOff", Range(0,1)) = 0.5
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }

        Pass{
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            sampler2D _MainTex;
            half4 _MainTex_ST;
            fixed _CutOff;
            fixed4 _Color;

            struct a2v{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                fixed2 uv : TEXCOORD2;
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                clip(texColor.a - _CutOff);
                fixed3 worldPos = normalize(i.worldPos);
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 albedo = texColor.rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldPos,worldNormal));
                return fixed4(ambient + diffuse , 1.0);
            }

            ENDCG

        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}
