Shader "MyShader/Zoom"
{
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" {}
	}

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
 
			struct a2v
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
 
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
 
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float2 _MousePos;                            //鼠标点击位置
			float _ZoomFactor;                           //缩放因子    
			float _CircleRadius;                         //圆的半径 
			float _CircleEdgeStrength;                   //圆边缘的强度   

			v2f vert(a2v v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
 
			fixed4 frag (v2f i) : SV_Target
			{
				//以高度为基准，获取屏幕宽高的缩放比值
				float2 widthHightScale = float2(_ScreenParams.x/_ScreenParams.y,1);

			    //1.鼠标点击的位置,c#代码传入
				float2 center = _MousePos; 
				
				//2.获取方向向量                              
				float2 dir = center - i.uv;     

				//3.点击位置和当前像素位置的距离
				float distance = length(dir * widthHightScale);

				//4.判断是否在圆上
				//fixed isZoomArea = 1 - step(_CircleRadius,distance);
				float isZoomArea = smoothstep(_CircleRadius + _CircleEdgeStrength,_CircleRadius,distance);
				
				//5.原始uv上整体叠加缩放值（_ZoomFactor是c#传入的缩放因子）                         
				fixed4 col = tex2D(_MainTex, i.uv + dir * _ZoomFactor * isZoomArea);  
				return col;
			}

		ENDCG
		}//Pass_end
	}
}