Shader "QualityShaders/Flipbook"
{
	Properties
	{
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_FlipBookSize ("Flipbook Size", Vector) = (1, 1, 0, 0)
		_Speed ("Animation Speed", Float) = 1.0
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
			Tags
			{
				"LightMode"="UniversalForward"
			}
			
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

			sampler2D _MainTex;

			CBUFFER_START(UnityPerMaterial)
			half4 _BaseColor;
			float4 _MainTex_ST;
			float2 _FlipBookSize;
			half _Speed;
			CBUFFER_END
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				float2 tileSize = float2(1, 1) / _FlipBookSize;
				float width = _FlipBookSize.x;
				float height = _FlipBookSize.y;
				float tileCount = width * height;
				float tileID = floor((_Time.y * _Speed) % tileCount);
				float tileX = (tileID % width) * tileSize.x;
				float tileY = (floor(tileID / width)) * tileSize.y;
				
				o.uv = float2(v.uv.x / width + tileX, v.uv.y / height + tileY);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDHLSL
		}
	}
}