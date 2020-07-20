// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DiffuseVertex"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1,1,1,1)
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

            struct a2v{
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed3 color : COLOR;
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                //
                fixed3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffue = _LightColor0.rgb * _Diffuse.rgb * saturate (dot(worldNormal,worldLight));
                o.color = ambient + diffue;
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                return fixed4(i.color , 1.0);
            }

            ENDCG

        }
    }
    FallBack "Diffuse"
}
