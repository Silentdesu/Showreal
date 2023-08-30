Shader "QualityShaders/PolarCoordinates"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _Center ("Center", Vector) = (0.5, 0.5, 0.0)
        _RadialScale ("Radial Scale", Float) = 1.0
        _LengthScale ("Light Scale", Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

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
            float4 _MainTex_ST;
            half4 _BaseColor;
            half2 _Center;
            float _RadialScale;
            float _LengthScale;
            CBUFFER_END

            float2 cartesianToPolar(float2 uv)
            {
                float2 offset = uv - _Center;
                float radius = length(offset) * 2;
                float angle = atan2(offset.x, offset.y) / (2.0F * PI);

                return float2(radius, angle);
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 radialUV = cartesianToPolar(i.uv);
                radialUV.x *= _RadialScale;
                radialUV.y *= _LengthScale;
                
                half4 col = tex2D(_MainTex, radialUV);
                return col * _BaseColor;
            }
            ENDHLSL
        }
    }
}