Shader "OnTheHorizon/LightMasks"
{
	Properties {
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque"}    
        Lighting Off   
        pass
        {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

	        struct VertInOut
	        {
	        	float4 color : COLOR; // Vertex color
	            float4 pos : POSITION; // Screen space coordinate
	            float3 ppos : TEXCOORD0; // Projector space coordinate
	            float3 pposS : TEXCOORD1; // Shifted projector space coordinate
	        };

			sampler2D _MaskTop;
			sampler2D _MaskLeft;
			sampler2D _MaskFront;
			sampler2D _MaskRamp;
			uniform float _MaskScale;
			fixed3 _ProjectorPos;
			fixed3 _PlayerMoved; // _MaskScale applied to it by script

	        // Transfer coordinates and vertex color to frag
	        VertInOut vert(VertInOut input, fixed3 normal : NORMAL)
	        {
	            VertInOut output;
	            output.pos = mul(UNITY_MATRIX_MVP,input.pos);
	            output.ppos = (mul(unity_ObjectToWorld, input.pos).xyz - _ProjectorPos) / _MaskScale;
	            output.pposS = output.ppos + fixed3(0.5,0.5,0.5);
	            output.color = float4(input.color.r, input.color.g, input.color.b, 1.0);
	            return output;
	        }
	        //
	        fixed4 frag(VertInOut input) : COLOR
	        {
	            fixed maskTop =  tex2D(_MaskTop, input.pposS.xz).r;
	            fixed maskLeft =  tex2D(_MaskLeft, fixed2(input.pposS.y, 1-input.pposS.z)).r;
	            fixed maskFront =  tex2D(_MaskFront, fixed2(input.pposS.xy)).r;
	            float maskRamp = tex2D(_MaskRamp, fixed2(length(input.ppos)*2, 0.5)).r;

	            // Use vertex color if is not zero, else 1
	            fixed colMul = tex2D(_MaskRamp, fixed2(1-(input.color.r+input.color.g+input.color.b)/3, 0.5)).r;
	            fixed3 col = input.color.rgb * colMul + fixed3(1,1,1) * (1-colMul);
	            return fixed4(col * min(min(maskTop, maskLeft), maskFront) * maskRamp, 1);
	        }
	        ENDCG
	    }
	}
	FallBack "Diffuse"
}