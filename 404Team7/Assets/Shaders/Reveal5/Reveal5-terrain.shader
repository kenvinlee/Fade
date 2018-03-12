Shader "Fade/Reveal5-Terrain" {
	Properties {
		// set by terrain engine
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		[HideInInspector] [Gamma] _Metallic0 ("Metallic 0", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic1 ("Metallic 1", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic2 ("Metallic 2", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic3 ("Metallic 3", Range(0.0, 1.0)) = 0.0
		[HideInInspector] _Smoothness0 ("Smoothness 0", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] _Smoothness1 ("Smoothness 1", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] _Smoothness2 ("Smoothness 2", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] _Smoothness3 ("Smoothness 3", Range(0.0, 1.0)) = 0.0

		// used in fallback on old cards & base map
		[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags {
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}

		
	// ------------------------------------------------------------
	// Surface shader code generated out of a CGPROGRAM block:
	

	// ---- forward rendering base pass:
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma multi_compile_fog
#pragma target 3.0
#pragma exclude_renderers gles
#pragma multi_compile __ _TERRAIN_NORMAL_MAP
#pragma multi_compile_fwdbase
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
// -------- variant for: <when no other keywords are defined>
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 4 texcoords actually used
//   float2 _Splat0
//   float2 _Splat1
//   float2 _Splat2
//   float2 _Splat3
#define UNITY_PASS_FORWARDBASE
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 31 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
		//#pragma surface surf Standard vertex:SplatmapVert finalcolor:SplatmapFinalColor finalgbuffer:SplatmapFinalGBuffer fullforwardshadows
		//#pragma multi_compile_fog
		//#pragma target 3.0
		// needs more than 8 texcoords
		//#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"

		//#pragma multi_compile __ _TERRAIN_NORMAL_MAP

		#define TERRAIN_STANDARD_SHADER
		#define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandard
		#include "TerrainSplatmapCommon.cginc"

		half _Metallic0;
		half _Metallic1;
		half _Metallic2;
		half _Metallic3;
		
		half _Smoothness0;
		half _Smoothness1;
		half _Smoothness2;
		half _Smoothness3;

		sampler2D _MaskTop;
		sampler2D _MaskLeft;
		sampler2D _MaskFront;
		sampler2D _MaskRamp;
		uniform float _MaskScale;
		fixed3 _ProjectorPos;
		fixed3 _PlayerMoved; // _MaskScale applied to it by script

		struct Output {
			fixed4 rt1 : SV_Target0;
			fixed4 rt2 : SV_Target1;
			fixed4 rt3 : SV_Target2;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 splat_control;
			half weight;
			fixed4 mixedDiffuse;
			half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);
			SplatmapMix(IN, defaultSmoothness, splat_control, weight, mixedDiffuse, o.Normal);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = weight;
			o.Smoothness = mixedDiffuse.a;
			o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));
		}
		

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
struct v2f_surf {
  float4 pos : SV_POSITION;
  float4 pack0 : TEXCOORD0; // _Splat0 _Splat1
  float4 pack1 : TEXCOORD1; // _Splat2 _Splat3
  half3 worldNormal : TEXCOORD2;
  float3 worldPos : TEXCOORD3;
  float2 custompack0 : TEXCOORD4; // tc_Control
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD5; // SH
  #endif
  SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  float3 pPos : TEXCOORD8;
  float4 nz : TEXCOORD9;
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
struct v2f_surf {
  float4 pos : SV_POSITION;
  float4 pack0 : TEXCOORD0; // _Splat0 _Splat1
  float4 pack1 : TEXCOORD1; // _Splat2 _Splat3
  half3 worldNormal : TEXCOORD2;
  float3 worldPos : TEXCOORD3;
  float2 custompack0 : TEXCOORD4; // tc_Control
  float4 lmap : TEXCOORD5;
  SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  fixed3 tSpace0 : TEXCOORD7;
  fixed3 tSpace1 : TEXCOORD8;
  fixed3 tSpace2 : TEXCOORD9;
  #endif
  float3 pPos : TEXCOORD10;
  float4 nz : TEXCOORD11;
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
float4 _Splat0_ST;
float4 _Splat1_ST;
float4 _Splat2_ST;
float4 _Splat3_ST;

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xy = customInputData.tc_Control;
  o.pos = UnityObjectToClipPos(v.vertex);
  o.pPos = (mul(unity_ObjectToWorld, v.vertex).xyz - _ProjectorPos) / _MaskScale;
  o.nz.xyz = COMPUTE_VIEW_NORMAL;
  o.nz.w = COMPUTE_DEPTH_01;
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
  o.pack0.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
  o.pack1.xy = TRANSFORM_TEX(v.texcoord, _Splat2);
  o.pack1.zw = TRANSFORM_TEX(v.texcoord, _Splat3);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
  return o;
}

// fragment shader
Output frag_surf (v2f_surf IN) {
  UNITY_SETUP_INSTANCE_ID(IN);
  // prepare and unpack data
  Input surfIN;
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.uv_Splat0.x = 1.0;
  surfIN.uv_Splat1.x = 1.0;
  surfIN.uv_Splat2.x = 1.0;
  surfIN.uv_Splat3.x = 1.0;
  surfIN.tc_Control.x = 1.0;
  surfIN.uv_Splat0 = IN.pack0.xy;
  surfIN.uv_Splat1 = IN.pack0.zw;
  surfIN.uv_Splat2 = IN.pack1.xy;
  surfIN.uv_Splat3 = IN.pack1.zw;
  surfIN.tc_Control = IN.custompack0.xy;
  float3 worldPos = IN.worldPos;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
  #else
  SurfaceOutputStandard o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  #if !defined(LIGHTMAP_ON)
      gi.light.color = _LightColor0.rgb;
      gi.light.dir = lightDir;
  #endif
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.worldViewDir = worldViewDir;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #if UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingStandard_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingStandard (o, worldViewDir, gi);
  SplatmapFinalColor (surfIN, o, c);
  UNITY_OPAQUE_ALPHA(c.a);

	fixed3 pPosS = IN.pPos + fixed3(0.5,0.5,0.5);
    fixed maskTop =  tex2D(_MaskTop, pPosS.xz).r;
    fixed maskLeft =  tex2D(_MaskLeft, fixed2(pPosS.y, 1-pPosS.z)).r;
    fixed maskFront =  tex2D(_MaskFront, fixed2(pPosS.xy)).r;
    float maskRamp = tex2D(_MaskRamp, fixed2(length(IN.pPos)*2, 0.5)).r;
	fixed3 col = min(min(maskTop, maskLeft), maskFront) * maskRamp;

	Output output;
	output.rt1 = c;
	output.rt2 = fixed4(col,0);
	output.rt3 = EncodeDepthNormal (IN.nz.w, IN.nz.xyz);
	return output;
}




ENDCG

}

	// ---- forward rendering additive lights pass:
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardAdd" }
		ZWrite Off Blend One One

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma multi_compile_fog
#pragma target 3.0
#pragma exclude_renderers gles
#pragma multi_compile __ _TERRAIN_NORMAL_MAP
#pragma multi_compile_fwdadd_fullshadows
#pragma skip_variants INSTANCING_ON
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
// -------- variant for: <when no other keywords are defined>
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: YES
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 4 texcoords actually used
//   float2 _Splat0
//   float2 _Splat1
//   float2 _Splat2
//   float2 _Splat3
#define UNITY_PASS_FORWARDADD
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 31 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
		//#pragma surface surf Standard vertex:SplatmapVert finalcolor:SplatmapFinalColor finalgbuffer:SplatmapFinalGBuffer fullforwardshadows
		//#pragma multi_compile_fog
		//#pragma target 3.0
		// needs more than 8 texcoords
		//#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"

		//#pragma multi_compile __ _TERRAIN_NORMAL_MAP

		#define TERRAIN_STANDARD_SHADER
		#define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandard
		#include "TerrainSplatmapCommon.cginc"

		half _Metallic0;
		half _Metallic1;
		half _Metallic2;
		half _Metallic3;
		sampler2D _Falloff;
		
		half _Smoothness0;
		half _Smoothness1;
		half _Smoothness2;
		half _Smoothness3;

		struct Output {
			fixed4 rt1 : SV_Target0;
			fixed4 rt2 : SV_Target1;
			fixed4 rt3 : SV_Target2;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 splat_control;
			half weight;
			fixed4 mixedDiffuse;
			half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);
			SplatmapMix(IN, defaultSmoothness, splat_control, weight, mixedDiffuse, o.Normal);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = weight;
//			o.Smoothness = mixedDiffuse.a;
			o.Smoothness = 0;  // Disable smoothness
			o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));
		}
		

// vertex-to-fragment interpolation data
struct v2f_surf {
  float4 pos : SV_POSITION;
  float4 pack0 : TEXCOORD0; // _Splat0 _Splat1
  float4 pack1 : TEXCOORD1; // _Splat2 _Splat3
  half3 worldNormal : TEXCOORD2;
  float3 worldPos : TEXCOORD3;
  float2 custompack0 : TEXCOORD4; // tc_Control
  SHADOW_COORDS(5)
};
float4 _Splat0_ST;
float4 _Splat1_ST;
float4 _Splat2_ST;
float4 _Splat3_ST;

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xy = customInputData.tc_Control;
  o.pos = UnityObjectToClipPos(v.vertex);
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
  o.pack0.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
  o.pack1.xy = TRANSFORM_TEX(v.texcoord, _Splat2);
  o.pack1.zw = TRANSFORM_TEX(v.texcoord, _Splat3);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos = worldPos;
  o.worldNormal = worldNormal;

  TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
  return o;
}

// fragment shader
Output frag_surf (v2f_surf IN) {
  // prepare and unpack data
  Input surfIN;
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.uv_Splat0.x = 1.0;
  surfIN.uv_Splat1.x = 1.0;
  surfIN.uv_Splat2.x = 1.0;
  surfIN.uv_Splat3.x = 1.0;
  surfIN.tc_Control.x = 1.0;
  surfIN.uv_Splat0 = IN.pack0.xy;
  surfIN.uv_Splat1 = IN.pack0.zw;
  surfIN.uv_Splat2 = IN.pack1.xy;
  surfIN.uv_Splat3 = IN.pack1.zw;
  surfIN.tc_Control = IN.custompack0.xy;
  float3 worldPos = IN.worldPos;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutputStandard o = (SurfaceOutputStandard)0;
  #else
  SurfaceOutputStandard o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Alpha = 0.0;
  o.Occlusion = 1.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o); 

  #ifdef POINT
	  unityShadowCoord3 lightCoord = mul(unity_WorldToLight, unityShadowCoord4(worldPos, 1)).xyz;
	  float attenNoshadow = (tex2D(_Falloff, dot(lightCoord, lightCoord).rr).r);
	  float atten = attenNoshadow * SHADOW_ATTENUATION(IN);
  #else
	  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos);
	  float attenNoshadow = atten;
  #endif


  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  #if !defined(LIGHTMAP_ON)
      gi.light.color = _LightColor0.rgb;
      gi.light.dir = lightDir;
  #endif
  gi.light.color *= atten;
  c += LightingStandard (o, worldViewDir, gi);
  c.a = 0.0;
  SplatmapFinalColor (surfIN, o, c);
  UNITY_OPAQUE_ALPHA(c.a);

	Output output;
	output.rt1 = c;
	output.rt2 = fixed4(0,0,0,attenNoshadow); // Forward add contributes nothing to rt2/3
	output.rt3 = fixed4(0,0,0,0);
	return output;
}




ENDCG

}
	// ---- end of surface shader generated code

#LINE 68

	}

	Fallback "Nature/Terrain/Diffuse"
}
