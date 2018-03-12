/// Uses the fully loaded (all channels) standard surface shader for compability reasons.
/// Additionally it replaces the light look up texture and mark the alpha channel of 
/// pixels white only if they are in some light.

/// Main camera renders with this shader.


Shader "OnTheHorizon/Reveal3" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_BumpDepth ("Bumpmap Depth", Range(-5,5)) = 1
		_Emission ("Emission (RGB)", Color) = (0,0,0,1)
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
//		_Falloff ("Light Falloff", 2D) = "white" {}
	}
	SubShader {
		Tags {"RenderType"="Opaque" }
//		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows finalcolor:finalCol vertex:vert keepalpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		#include "UnityPBSLighting.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;
		uniform float4 _Normal_ST;
		sampler2D _Falloff;

		half _BumpDepth;
		half _Glossiness;
		half _Metallic;
		half4 _Emission;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float4 pos;
		};

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o)
			o.pos = v.vertex;
		}

//		half4 LightingReveal (SurfaceOutputStandard s, half3 viewDir, UnityGI gi) {
//			half4 c = LightingStandard (s, viewDir, gi);
//			c.a = s.Alpha;
//			return c;
//		}
//		void LightingReveal_GI ( SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi) {
//			LightingStandard_GI ( s, data, gi);
//		}

		void finalCol (Input IN, SurfaceOutputStandard s, inout fixed4 color) {
			color.a = s.Alpha;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = lerp(UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)), fixed3(0,0,1), -_BumpDepth+1);
			o.Emission = _Emission.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 0;

			#ifdef POINT
				float4 worldPos = mul(unity_ObjectToWorld, IN.pos);
				float3 LightCoord = mul(unity_WorldToLight, worldPos).xyz;
				o.Alpha = tex2D(_Falloff, dot(LightCoord, LightCoord).xx).UNITY_ATTEN_CHANNEL;
				_LightTexture0 = _Falloff;
			#endif
		}
		ENDCG
	}
	FallBack "Diffuse"
}
