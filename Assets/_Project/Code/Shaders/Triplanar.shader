Shader "QualityShaders/Triplanar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1, 1, 1, 1)
        _Tile ("Texture Tiling", Float) = 1.0
        _BlendPower ("Triplanar Blending", Float) = 10.0
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldSpace : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _BaseColor;
            float _Tile;
            float _BlendPower;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.worldSpace = TransformObjectToWorld(v.vertex.xyz);
                o.normal = TransformObjectToWorldNormal(v.normal);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 xAxisUV = i.worldSpace.zy * _Tile;
                float2 yAxisUV = i.worldSpace.xz * _Tile;
                float2 zAxisUV = i.worldSpace.xy * _Tile;

                float4 xSample = tex2D(_MainTex, xAxisUV);
                float4 ySample = tex2D(_MainTex, yAxisUV);
                float4 zSample = tex2D(_MainTex, zAxisUV);

                float3 weights = pow(abs(i.normal), _BlendPower);
                weights /= (weights.x + weights.y + weights.z);

                half4 color = xSample * weights.x + ySample * weights.y + zSample * weights.z;
                
                return color;
            }
            ENDHLSL
        }
    }
}
