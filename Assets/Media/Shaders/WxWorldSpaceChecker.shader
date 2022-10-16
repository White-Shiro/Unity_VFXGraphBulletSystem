Shader "WxCore/WxWorldSpaceChecker"
{
	Properties
	{
		_Color0("Color0", Color) = (0.6, 0.75, 0.9, 1)
		_Color1("Color1", Color) = (0, 0, 0, 1)
		_CheckSize("CheckSize", Range(0.1, 10)) = 1
		_LineWidth("LineWidth", Range(0.01, 1)) = 0.02
	}
	SubShader
	{
		Tags {
			"RenderType"		= "Opaque"
			// "DisableBatching" 	= "True"
			"RenderPipeline" = "UniversalPipeline"
		}

		Pass
		{
			// Cull Off
			
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD4;
			};

			float4 _Color0;
			float4 _Color1;
			float _CheckSize;
			float _LineWidth;

			v2f vert (appdata i)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.worldPos = mul(unity_ObjectToWorld, i.vertex);
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				float3 worldNormal = normalize(cross(ddx(i.worldPos), ddy(i.worldPos)));

				float dx = abs(dot(worldNormal, float3(1,0,0)));
				float dy = abs(dot(worldNormal, float3(0,1,0)));
				float dz = abs(dot(worldNormal, float3(0,0,1)));

				// pick the axis
				float2 pt;
				if (dx > dy) {
					if (dx > dz) {
						pt = i.worldPos.yz;
					} else {
						pt = i.worldPos.xy;
					}
				} else {
					if (dy > dz) {
						pt = i.worldPos.xz;
					} else {
						pt = i.worldPos.xy;
					}
				}

				float2 w = pt / _CheckSize;
				w = fmod(abs(w), 1); // - _LineWidth * 0.5;
				w = clamp(w, 0.5 - _LineWidth, 0.5 + _LineWidth);
				w = 1 - abs(w - 0.5) / _LineWidth;
				float s = saturate(w.x + w.y);
				
				float4 col = lerp(_Color0, _Color1, s);
				return col;
			}
			ENDCG
		}
	}
}
