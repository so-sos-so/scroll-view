// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/NormalMatSpace"
{
    Properties
    {
        _Specular ("Specular", Color ) = (1,1,1,1)
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _MainTex ("Main Tex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BumpMap ("Bump Map", 2D) = "bump" {}
        _BumpScale ("Bump Scale", float) = 1.0
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float _BumpScale;
            fixed4 _Specular;
            float _Gloss;

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 tangent : TANGENT;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 TtoW0: TEXCOORD1;
                float4 TtoW1: TEXCOORD2;
                float4 TtoW2: TEXCOORD3;
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET{

                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);

                fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                bump *= _BumpScale;
                bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
                bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
                

                fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

                fixed3 worldLight = normalize( _WorldSpaceLightPos0.xyz);
                //fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(bump, lightDir));

                fixed3 halfDir = normalize(lightDir + viewDir);
                fixed3 emmi = albedo * _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(bump, halfDir)) ,_Gloss);
                fixed3 color = ambient + emmi + diffuse;
                return fixed4(color,1.0);
            }


            ENDCG
        }
    }
    Fallback "Diffuse"
}