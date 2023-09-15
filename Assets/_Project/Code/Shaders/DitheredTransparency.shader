Shader "QualityShaders/DitheredTransparency"
{
    Properties
    {
        [MainColor] _Color ("Color", Color) = (1, 1, 1, 1)
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "AlphaTest"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 300

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }
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
                float4 positionSS : TEXCOORD1;
            };

            sampler2D _MainTex;
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.positionSS = ComputeScreenPos(v.positionOS);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                const half4 mainTexSample = tex2D(_MainTex, i.uv);
                const half4 outputColor = mainTexSample * _Color;
                const float2 screenUVs = i.positionSS.xy / i.positionSS.w * _ScreenParams.xy;

                const float ditherThresholds[16] =
                {
                    16.0 / 17.0, 8.0 / 17.0, 14.0 / 17.0, 6.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0, 2.0 / 17.0, 10.0 / 17.0,
                    13.0 / 17.0, 5.0 / 17.0, 15.0 / 17.0, 7.0 / 17.0,
                    1.0 / 17.0, 9.0 / 17.0, 3.0 / 17.0, 11.0 / 17.0
                };

                uint index = (uint(screenUVs.x) % 4) * 4 + uint(screenUVs.y) % 4;
                const float threshold = ditherThresholds[index];

                if (outputColor.a < threshold) discard;
                
                return outputColor;
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Unlit"
}