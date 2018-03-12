// Adapted From Unity Standard Assets
/// Take an X shaped sample around each pixel for their normals. If the normal has too big a difference,
/// mark the center pixel as an edge/white
Shader "OnTheHorizon/EdgeDetect" { 
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}  // Input for post processing
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[5] : TEXCOORD0;
	};

	sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	half4 _MainTex_ST;

	sampler2D _CameraDepthNormalsTexture;
	half4 _CameraDepthNormalsTexture_ST;

	uniform half4 _Sensitivity; 
	uniform half _FaceIntensity;
	uniform half _BorderIntensity;
	uniform half _SampleDistance;
	
	inline half CheckSame (half2 centerNormal, float centerDepth, half4 theSample)
	{
		// difference in normals
		// do not bother decoding normals - there's no need here
		half2 diff = abs(centerNormal - theSample.xy) * _Sensitivity.y;
		int isSameNormal = (diff.x + diff.y) * _Sensitivity.y < 0.1;
		// difference in depth
		float sampleDepth = DecodeFloatRG (theSample.zw);
		float zdiff = abs(centerDepth-sampleDepth);
		// scale the required threshold by the distance
		int isSameDepth = zdiff * _Sensitivity.x < 0.09 * centerDepth;
	
		// return:
		// 1 - if normals and depth are similar enough
		// 0 - otherwise
		return isSameNormal * isSameDepth ? 1.0 : 0.0;
	}	
		
	v2f vertRobert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv[0] = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1-uv.y;
		#endif

		// Take X shaped samples
		o.uv[1] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(1,1) * _SampleDistance, _MainTex_ST);
		o.uv[2] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(-1,-1) * _SampleDistance, _MainTex_ST);
		o.uv[3] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(-1,1) * _SampleDistance, _MainTex_ST);
		o.uv[4] = UnityStereoScreenSpaceUVAdjust(uv + _MainTex_TexelSize.xy * half2(1,-1) * _SampleDistance, _MainTex_ST);
				 
		return o;
	} 

	half4 fragRobert(v2f i) : SV_Target {	
		// Considering possible if-else performance hit, if statement has been removed.			
		half4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1].xy);
		half4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2].xy);
		half4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3].xy);
		half4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4].xy);

		half edge = 1.0;
		edge *= CheckSame(sample1.xy, DecodeFloatRG(sample1.zw), sample2);
		edge *= CheckSame(sample3.xy, DecodeFloatRG(sample3.zw), sample4);

		return (_BorderIntensity + _FaceIntensity - edge) * tex2D(_MainTex, i.uv[0]) ;	
	}
	
	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vertRobert
      #pragma fragment fragRobert
      ENDCG
  }
}

Fallback off
	
} // shader
