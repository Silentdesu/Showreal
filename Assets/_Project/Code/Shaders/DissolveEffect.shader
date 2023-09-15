Shader "QualityShaders/DissolveEffect"
{
    Properties
    {
        [MainColor] _Color ("Color", Color) = (1, 1, 1, 1)
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        
        _CutoffHeight ("Cutoff Height", Range(-1, 1)) = 0.0
        _NoiseScale ("Noise Scale", Float) = 20
        _NoiseStrength ("Noise Strength", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "AlphaTest"
            "Renderpipeline" = "UniversalPipeline"
        }
        LOD 300

        Pass
        {
            Cull Off
            
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float4 positionOS : TEXCOORD1;
            };

            sampler2D _MainTex;
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half _CutoffHeight;
                half _NoiseScale;
                half _NoiseStrength;
            CBUFFER_END

            float2 generateDir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float generateNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);

                float d00 = dot(generateDir(ip), fp);
                float d01 = dot(generateDir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(generateDir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(generateDir(ip + float2(1, 1)), fp - float2(1, 1));

                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            float gradientNoise(float2 UV, float scale)
            {
                return generateNoise(UV * scale) * 2.0f;
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                o.positionOS = v.positionOS;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                const float noiseSample = gradientNoise(i.uv, _NoiseScale) * _NoiseStrength;
                const float noisyPosition = i.positionOS.y + noiseSample;

                if (noisyPosition > _CutoffHeight) discard;

                const half4 col = tex2D(_MainTex, i.uv);
                return col * _Color;
            }
            ENDHLSL
        }
    }
}