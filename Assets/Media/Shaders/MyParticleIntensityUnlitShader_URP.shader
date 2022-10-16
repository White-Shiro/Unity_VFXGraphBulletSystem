Shader "MyShaders/Particle/MyParticleIntensityUnlitShader_URP" {

	Properties {
		[MainTexture] _BaseMap("Base Map", 2D) = "white" {}
		[HDR] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
	}
	
	SubShader {
		Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline" }

		Pass {
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes {
				float4 positionOS   : POSITION;
				float2 uv           : TEXCOORD0;
				float4 color		: COLOR;
			};

			struct Varyings {
				float4 positionHCS  : SV_POSITION;
				float2 uv           : TEXCOORD0;
				float4 color		: COLOR;
			};

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			CBUFFER_START(UnityPerMaterial)
				half4	_BaseColor;
				float4	_BaseMap_ST;


			CBUFFER_END


			Varyings vert(Attributes IN) {
				Varyings OUT;
				OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
				OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
				OUT.color = IN.color;
				return OUT;
			}

			float4 frag(Varyings IN) : SV_Target {
				float4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
				color *= _BaseColor * IN.color;
				return color;
			}

			ENDHLSL
		}
	}
}
