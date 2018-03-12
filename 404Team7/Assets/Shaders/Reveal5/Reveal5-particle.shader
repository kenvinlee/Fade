Shader "Fade/Reveal5-Particle" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

	SubShader {
		Pass {
		Blend SrcAlpha OneMinusSrcAlpha
//		ColorMask RGB
		Cull Off Lighting Off ZWrite Off
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
				UNITY_VERTEX_OUTPUT_STEREO
			};

			struct Output {
				fixed4 rt1 : SV_Target0;
				fixed4 rt2 : SV_Target1;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color * _TintColor;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			
			Output frag (v2f i)
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				fixed4 col = 2.0f * i.color * tex2D(_MainTex, i.texcoord);

				Output output;
				output.rt1 = fixed4(col);
				output.rt2 = fixed4(0,0,0,0);
				return output;
			}
			ENDCG 
		}
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardAdd" }
			Blend One One
			Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd
			
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				float3 wPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Falloff;
			sampler2D _MainTex;
			
			v2f vert (float4 vertex : POSITION, float2 texcoord : TEXCOORD0)
			{
				v2f o;
				o.wPos = mul(unity_ObjectToWorld, vertex);
				o.vertex = UnityObjectToClipPos(vertex);
				o.texcoord = texcoord;
				return o;
			}

			struct Output {
				fixed4 rt1 : SV_Target0;
				fixed4 rt2 : SV_Target1;
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
				output.rt2 = fixed4(0,0,0,attenNoshadow * tex2D(_MainTex, i.texcoord).a);
				return output;
			}
			ENDCG
		}
	}	
}
}
