Shader "Custom/卡通"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DiffuseColor ("Diffuse", color) = (1,1,1,1)
        _Spector ("Spector", Range(0.1, 50)) = 0.5
    }
    SubShader
    {
        Pass{
            Cull Front
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _DiffuseColor;
            half _Spector;
            
            struct o2v{
                float4 vertex : POSITION;
                fixed2 texcoord : TEXCOORD0;
                fixed3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };
            
            v2f vert(o2v v)
            {
                v2f o;
                v.vertex.xyz += v.normal * 0.02;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                return fixed4(0,0,0,1);
            }
       
            ENDCG
        }

        Pass{
            Tags { "LightMode" = "ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _DiffuseColor;
            half _Spector;
            
            struct o2v{
                float4 vertex : POSITION;
                fixed2 texcoord : TEXCOORD0;
                fixed3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                fixed3 worldNormal : TEXCOORD1;
            };

            v2f vert(o2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.worldNormal = normalize( UnityObjectToWorldNormal(v.normal));
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                fixed3 tex = tex2D(_MainTex, i.uv);
                fixed3 ambient = normalize( UNITY_LIGHTMODEL_AMBIENT.xyz) * tex;
                float dotResult = 0.5 + 0.5 * dot(i.worldNormal, normalize(_WorldSpaceLightPos0.xyz));
                if(dotResult > 0.9){
                    dotResult = 1;
                }else if(dotResult > 0.6){
                    dotResult = 0.6;
                }else{
                    dotResult = 0;
                }
                fixed3 diffuse = _DiffuseColor * _LightColor0 * saturate(dotResult) * tex;
                //fixed3 spector =  _LightColor0 * pow(saturate(dot(i.worldNormal,  ((_WorldSpaceCameraPos + i.worldLightDir) / 2))),_Spector);
                //fixed3 color = diffuse + ambient + spector;
                fixed3 color = diffuse + ambient;
                return fixed4( color,1);
            }
            ENDCG
        }


   }
    FallBack "Diffuse"
}
