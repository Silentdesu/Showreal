Shader "QualityShaders/DissolveWorld"
{
    Properties
    {
        [MainColor] _Color ("Color", Color) = (1, 1, 1, 1)
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}

        _NoiseScale ("Noise Scale", Float) = 0.5
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "Renderpipeline" = "UniversalPipeline"
        }
        LOD 300

        Pass
        {
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
                float4 positionWS : TEXCOORD1;
            };

            sampler2D _MainTex;
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half _NoiseScale;
                half _NoiseStrength;
                float3 _PlaneOrigin;
                float3 _PlaneNormal;
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
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.positionWS = mul(unity_ObjectToWorld, v.positionOS);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                const float noiseSample = gradientNoise(i.uv, _NoiseScale) * _NoiseStrength;
                const float3 noisyPosition = i.positionWS.xyz + _PlaneNormal * noiseSample;
                const float3 offset = noisyPosition - _PlaneOrigin;

                if (dot(offset, _PlaneNormal) > 0.0f) discard;

                const half4 col = tex2D(_MainTex, i.uv);
                return col * _Color;
            }
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Unlit/DEPTHONLY"
    }
}