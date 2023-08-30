Shader "QualityShaders/ParticleFlipbook"
{
	Properties
	{
		_MainTex ("Texture", 3D) = "white" {}
		_BaseColor ("Color", Color) = (1, 1, 1, 1)
		_Speed ("Animation speed", Float) = 1.0
	}
	SubShader
	{
		Tags 
		{
			 "RenderType"="Opaque"
			 "Queue"="Geometry"
			 "RenderPipeline"="UniversalPipeline" 
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler3D _MainTex;
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			half4 _BaseColor;
			float _Speed;
			CBUFFER_END
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float3 animUV = float3(i.uv, _Time.y * _Speed);
				float4 textureSample = tex3D(_MainTex, animUV);
				
				return textureSample * _BaseColor;
			}
			ENDHLSL
		}
	}
}