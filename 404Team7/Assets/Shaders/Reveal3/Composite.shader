/// Blend outputs from both cameras.
/// Basic alpha blend (Main camera output alpha/light range)

Shader "OnTheHorizon/Composite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {} // Main camera normal render
//		_ChildTex("PreProcessTexture", 2D) = "black" {} // Second camera rendering white borders
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 childuv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};
			
			sampler2D _MainTex;
			sampler2D _ChildTex;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.childuv = v.uv;
//				#if UNITY_UV_STARTS_AT_TOP
//					o.childuv.y = 1-v.uv.y;
//				#else
//				#endif
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 childCol = tex2D(_ChildTex, i.childuv);

				return fixed4 (col.rgb * col.a + childCol.rgb * (1-col.a), 1);
//return childCol;
			}
			ENDCG
		}
	}
}
