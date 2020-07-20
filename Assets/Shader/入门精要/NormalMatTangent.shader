Shader "Custom/NormalMat"
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
                float3 lightDir : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
                TANGENT_SPACE_ROTATION;
                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET{
                fixed3 tangentLightDir = normalize(i.lightDir);
                fixed3 tangentViewDir = normalize(i.viewDir);

                fixed4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                fixed3 tangentNormal = UnpackNormal(packedNormal);
                tangentNormal *= _BumpScale;
                tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
                
                fixed3 worldLight = normalize( _WorldSpaceLightPos0.xyz);
                //fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));

                fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
                fixed3 emmi = albedo * _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(tangentNormal, halfDir)) ,_Gloss);
                fixed3 color = ambient + emmi + diffuse;
                return fixed4(color,1.0);
            }


            ENDCG
        }
    }
    Fallback "Diffuse"
}