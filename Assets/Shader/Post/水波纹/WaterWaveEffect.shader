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
            float _timeFactor;
            float2 _screenCenter;
            float _waveDistance;
            float _waveWidth;
    
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
                half2 uv = _screenCenter - f.uv;
                uv = uv * float2(_ScreenParams.x / _ScreenParams.y , 1);
                float dis = sqrt(dot(uv,uv));
                //sin(波纹频率)  sin值*value  是波峰
                float sinFactor = sin(dis * _distanceFactor * 60 + _Time.y * _timeFactor) * 0.05;
                //_waveWidth 运动点周围能显示多少宽度的wave
                float show = clamp(_waveWidth - abs(_waveDistance - dis),0,1);
                uv = normalize(uv);
                f.uv = uv * sinFactor * show + f.uv;
                fixed4 color = tex2D(_MainTex, f.uv);
                return color;
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"
}