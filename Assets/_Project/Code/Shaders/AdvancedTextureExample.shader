Shader "QualityShaders/AdvancedTextureExample"
{
	Properties
	{
		_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_Rotation ("Rotation Amount", Float) = 0.0
		_Center ("Rotation Center", Vector) = (0, 0, 0, 0)
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
				"Lightmode"="UniversalForward"
			}
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
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
			float4 _MainTex_ST;
			half4 _BaseColor;
			float _Rotation;
			float2 _Center;
			CBUFFER_END
			
			v2f vert (appdata v)
			{
				float c = cos(_Rotation);
				float s = sin(_Rotation);
				float2x2 rotMatrix = float2x2(c, -s, s, c);
				
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv -= _Center;
				o.uv = mul(rotMatrix, o.uv);
				o.uv += _Center;	
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
				return col * _BaseColor;
			}
			ENDHLSL
		}
	}
}