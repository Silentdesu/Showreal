Shader "QualityShaders/SepiaTone"
{
    Properties {}

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 300

        Pass
        {
            Name "SepiaTonePass"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 positionSS : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionSS = ComputeScreenPos(o.positionCS);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                const float3x3 sepia = float3x3
                (
                    0.393f, 0.349f, 0.272f,
                    0.769f, 0.686f, 0.534f,
                    0.189f, 0.168f, 0.131f
                );

                const float2 screenUVs = i.positionSS.xy / i.positionSS.w;
                const float3 sceneColor = SampleSceneColor(screenUVs);
                const half3 outputColor = mul(sceneColor, sepia);
                
                return half4(outputColor, 1.0f);
            }
            ENDHLSL
        }
    }
}