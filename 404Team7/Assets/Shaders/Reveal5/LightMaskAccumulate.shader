Shader "OnTheHorizon/LightMaskAccumulate"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	CGINCLUDE
	
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert (appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;
	sampler2D _LastFrame;
	sampler2D _MaskRamp;
	float4 _MainTex_ST;
	float3 _PlayerMoved;
	float _DimRate;

	float4 fragTop (v2f i) : SV_Target
	{
	    float maskRamp = tex2D(_MaskRamp, fixed2(length(i.uv-fixed2(0.5,0.5))*2, 0.5)).r;
		return max(tex2D(_MainTex, i.uv),  tex2D(_LastFrame, i.uv+_PlayerMoved.xz)-float4(1,1,1,0)*_DimRate)* maskRamp;
	}

	float4 fragLeft (v2f i) : SV_Target
	{
	    float maskRamp = tex2D(_MaskRamp, fixed2(length(i.uv-fixed2(0.5,0.5))*2, 0.5)).r;
		return max(tex2D(_MainTex, i.uv), tex2D(_LastFrame, i.uv+float2(_PlayerMoved.y, -_PlayerMoved.z))-float4(1,1,1,0)*_DimRate)* maskRamp;
	}

	float4 fragFront (v2f i) : SV_Target
	{
	    float maskRamp = tex2D(_MaskRamp, fixed2(length(i.uv-fixed2(0.5,0.5))*2, 0.5)).r;
		return max(tex2D(_MainTex, i.uv), tex2D(_LastFrame, i.uv+_PlayerMoved.xy)-float4(1,1,1,0)*_DimRate)* maskRamp;
	}
	ENDCG

Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragTop
      ENDCG
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragLeft
      ENDCG
  }
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragFront
      ENDCG
  }
}
}
