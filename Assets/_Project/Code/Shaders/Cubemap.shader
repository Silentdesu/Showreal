Shader "QualityShaders/Cubemap"
{
	Properties
	{
		_Cube ("Cube", CUBE) = "whote" {}
	}
	SubShader
	{
		Tags 
		{ 
			"RenderType"="Opaque"
			"Queue"="Geometry" 
		}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD0;
			};

			samplerCUBE _Cube;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.normal = TransformObjectToWorldNormal(v.normal);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half4 col = texCUBE(_Cube, i.normal);
				return col;
			}
			ENDHLSL
		}
	}
}