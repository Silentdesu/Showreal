Shader "QualityShaders/Silhouette"
{
    Properties
    {
        _ForegroundColor ("Foreground Color", Color) = (1, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 positionSS : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _ForegroundColor;
            half4 _BackgroundColor;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.positionSS = ComputeScreenPos(o.positionCS);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 screenUVs = i.positionSS.xy / i.positionSS.w;
                float rawDepth = SampleSceneDepth(screenUVs);
                float scene01Depth = Linear01Depth(rawDepth, _ZBufferParams);
                float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);

                half4 col = lerp(_ForegroundColor, _BackgroundColor, scene01Depth);

                return col;
            }
            ENDHLSL
        }

    }
    Fallback "Universal Render Pipeline/Diffuse"
}