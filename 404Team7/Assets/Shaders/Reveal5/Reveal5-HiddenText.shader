// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Fade/Reveal5-HiddenText"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Geometry+50" }
		LOD 200

		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			Blend One One
			ZWrite Off Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
  				float3 ppos : TEXCOORD1; //Mask projector space position
  				float3 pposs : TEXCOORD2; //Mask projector space position shifted
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

			sampler2D _MaskTop;
			sampler2D _MaskLeft;
			sampler2D _MaskFront;
			sampler2D _MaskRamp;
			uniform float _MaskScale;
			fixed3 _ProjectorPos;
			
			v2f vert (float2 uv : TEXCOORD0, float4 vertex : POSITION)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.uv = TRANSFORM_TEX(uv, _MainTex);
            	fixed3 projectorSpacePos = (mul(unity_ObjectToWorld, vertex).xyz - _ProjectorPos) / _MaskScale;
            	o.ppos = projectorSpacePos;
            	o.pposs = projectorSpacePos + fixed3(0.5,0.5,0.5);
				return o;
			}

			struct Output {
				fixed4 rt1 : SV_Target0;
				fixed4 rt2 : SV_Target1;
				fixed4 rt3 : SV_Target2;
				fixed4 rt4 : SV_Target3;
			};
			
			Output frag (v2f i)
			{
				// sample the texture
				fixed4 c = tex2D(_MainTex, i.uv) *_Color;

				// sample the light masks
				fixed3 pPosS = i.pposs;
			    fixed maskTop =  tex2D(_MaskTop, pPosS.xz).r;
			    fixed maskLeft =  tex2D(_MaskLeft, fixed2(pPosS.y, 1-pPosS.z)).r;
			    fixed maskFront =  tex2D(_MaskFront, fixed2(pPosS.xy)).r;
			    float maskRamp = tex2D(_MaskRamp, fixed2(length(i.ppos)*2, 0.5)).r;
				fixed mask = min(min(maskTop, maskLeft), maskFront) * maskRamp;

				Output output;
				output.rt1 = fixed4(0,0,0,0);
				output.rt2 = fixed4(0,0,0,0);
				output.rt3 = fixed4(0,0,0,0);
				output.rt4 = fixed4(c.rgb, c.a * mask); // Only write to rt4.
				return output;
			}
			ENDCG
		}

		Pass
		{
			// Subtract light atten from alpha when lit.
			Name "FORWARD"
			Tags { "LightMode" = "ForwardAdd" }
			ZWrite Off Cull Off Blend One One
			BlendOp RevSub

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd
			
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			struct v2f
			{
				float3 wPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Falloff;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.wPos = mul(unity_ObjectToWorld, vertex);
				o.vertex = UnityObjectToClipPos(vertex);
				return o;
			}

			struct Output {
				fixed4 rt1 : SV_Target0;
				fixed4 rt2 : SV_Target1;
				fixed4 rt3 : SV_Target2;
				fixed4 rt4 : SV_Target3;
			};
			
			Output frag (v2f i)
			{
				#ifdef POINT
				  unityShadowCoord3 lightCoord = mul(unity_WorldToLight, unityShadowCoord4(i.wPos, 1)).xyz;
				  float attenNoshadow = (tex2D(_Falloff, dot(lightCoord, lightCoord).rr).r);
				#else
				  UNITY_LIGHT_ATTENUATION(atten, i, i.wPos);
				  float attenNoshadow = atten;
				#endif

				Output output;
				output.rt1 = fixed4(0,0,0,0);
				output.rt2 = fixed4(0,0,0,0);
				output.rt3 = fixed4(0,0,0,0);
				output.rt4 = fixed4(0,0,0,attenNoshadow);
				return output;
			}
			ENDCG
		}
	}
}
