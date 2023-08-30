Shader "QualityShaders/CubemapReflection"
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
				float3 reflection : TEXCOORD0;
			};

			samplerCUBE _Cube;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);

				float3 normal = TransformObjectToWorldNormal(v.normal);
				float3 pos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 viewDir = GetWorldSpaceNormalizeViewDir(pos);

				o.reflection = reflect(-viewDir, normal);
				
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half4 col = texCUBE(_Cube, i.reflection);
				return col;
			}
			ENDHLSL
		}
	}
}
