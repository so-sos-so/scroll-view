Shader "Unlit/GrabPass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius",Range(1,5))=1
	}
	SubShader
	{
		Tags 
        {
            "RenderType"="Transparent"
        }

        
        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass{}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment HorFrag
			ENDCG
		}

        GrabPass{}

        Pass
        {
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment VertFrag
			ENDCG
        }

       
	}

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _GrabTexture;
	float4 _MainTex_ST;
    float4 _GrabTexture_TexelSize;
    int _Radius;


    struct appdata
	{
		float4 vertex : POSITION;
        float4 color : COLOR;
	};

	struct v2f
	{
        float4 color : COLOR;
		float2 screen_uv : TEXCOORD;
		float4 vertex : SV_POSITION;
	};

    static const int CenterIndex=3;

    static const half GaussWeight[7] =
    {0.0205,0.0855,0.232,0.324,0.232,0.0855,0.0205,};

    

    v2f vert (appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);

        float4 screenPos=ComputeGrabScreenPos(o.vertex);
		o.screen_uv = screenPos.xy/screenPos.w;

        o.color=v.color;
		return o;
	}

    fixed4 HorFrag (v2f input) : SV_Target
    {
        float xOffset=_GrabTexture_TexelSize.x*_Radius;
		
        fixed4 col=tex2D(_GrabTexture, input.screen_uv)*GaussWeight[CenterIndex];
        for(int i=1;i<=3;i++)
        {
            float2 rightUV=input.screen_uv+float2(xOffset,0)*i;
            float2 leftUV=input.screen_uv-float2(xOffset,0)*i;
			
            col+=tex2D(_GrabTexture,rightUV)*GaussWeight[CenterIndex+i];
            col+=tex2D(_GrabTexture,leftUV)*GaussWeight[CenterIndex-i];
        }
		
		col = fixed4(col.rgb, 1);
    	
    	return col;    	
    }

    fixed4 VertFrag (v2f input) : SV_Target
    {
        float yOffset=_GrabTexture_TexelSize.y*_Radius;
        fixed4 col=tex2D(_GrabTexture, input.screen_uv)*GaussWeight[CenterIndex];
        for(int i=1;i<=3;i++)
        {
            float2 topUV=input.screen_uv+float2(0,yOffset)*i;
            float2 downUV=input.screen_uv-float2(0,yOffset)*i;
        
            col+=tex2D(_GrabTexture,topUV)*GaussWeight[CenterIndex+i];
            col+=tex2D(_GrabTexture,downUV)*GaussWeight[CenterIndex-i];
        }
        
		col = fixed4(col.rgb, 1);

        return col*input.color;    	    
    }


    ENDCG

    FallBack Off

}
