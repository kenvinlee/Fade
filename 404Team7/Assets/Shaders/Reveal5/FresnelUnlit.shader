Shader "Fade/FresnelUnlit"
{
	Properties
	{}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				fixed4 vertex : POSITION;
				fixed u : TEXCOORD0;
			};

			sampler2D _Falloff;
			
			v2f vert (fixed4 vertex : POSITION, fixed3 normal : NORMAL)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.u = length(normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normal)).xy);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_Falloff, fixed2(i.u, 0.5));
			}
			ENDCG
		}
	}
}
