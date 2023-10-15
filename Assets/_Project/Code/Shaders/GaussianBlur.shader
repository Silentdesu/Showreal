Shader "QualityShaders/GaussianBlur"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}

    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 300

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        static const float e = 2.71828f;

        float gaussianBlur(const int x, const float sigma)
        {
            const float twoSigmaSqr = 2 * sigma * sigma;
            return (1 / sqrt(PI * twoSigmaSqr)) *
                pow(e, -(x * x) / (2 * twoSigmaSqr));
        }

        struct appdata
        {
            float4 positionCS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 positionOS : SV_POSITION;
        };

        sampler2D _MainTex;

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            uint _KernelSize;
        CBUFFER_END

        v2f vert(appdata v)
        {
            v2f o;
            o.positionOS = TransformObjectToHClip(v.positionCS.xyz);
            o.uv = v.uv;
            return o;
        }
        ENDHLSL

        Pass
        {
            Name "Horizontal"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragHorizontal

            half4 fragHorizontal(v2f i) : SV_TARGET
            {
                half3 col = half3(0.0f, 0.0f, 0.0f);
                half kernelSum = 0.0f;
                const half sigma = _KernelSize / 8.0f;

                const int upper = ((_KernelSize - 1) / 2);
                const int lower = -upper;

                for (int x = lower; x <= upper; ++x)
                {
                    const float gauss = gaussianBlur(x, sigma);
                    kernelSum += gauss;
                    const float2 uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);
                    col += max(0, gauss * tex2D(_MainTex, uv).xyz);
                }

                col /= kernelSum;
                
                return half4(col, 1.0f);
            }
            ENDHLSL
        }
        Pass 
        {
            Name "Vertical"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragVertical

            half4 fragVertical(v2f i) : SV_TARGET
            {
                half3 col = half3(0.0f, 0.0f, 0.0f);
                half kernelSum = 0.0f;
                const half sigma = _KernelSize / 8.0f;

                const int upper = ((_KernelSize - 1) / 2);
                const int lower = -upper;

                for (int x = lower; x <= upper; ++x)
                {
                    const float gauss = gaussianBlur(x, sigma);
                    kernelSum += gauss;
                    const float2 uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);
                    col += max(0, gauss * tex2D(_MainTex, uv).xyz);
                }

                col /= kernelSum;
                
                return half4(col, 1.0f);
            }
            
            ENDHLSL
        }
    }
Fallback "Universal Render Pipeline/Simple Lit"
}