Shader "QualityShaders/AlphaCutout"
{
    Properties
    {
        [MainColor] _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        _ClipThreshold ("Alpha Clip Threshold", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "AlphaTest"
            "Renderpipeline" = "Universalpipeline"
        }
        LOD 300

        Pass
        {
            
            Tags { "LightMode" = "UniversalForward" }
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
                half _ClipThreshold;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 textureSample = tex2D(_MainTex, i.uv);
                half4 outputColor = textureSample * _BaseColor;
                // AlphaDiscard(col.a, _ClipThreshold);
                if (outputColor.a < _ClipThreshold) discard;
                return outputColor;
            }
            ENDHLSL
        }
    }
}