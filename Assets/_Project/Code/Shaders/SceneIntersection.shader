Shader "QualityShaders/SceneIntersection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0, 0, 0, 1)
        _IntersectColor ("Intersect Color", Color) = (1, 1, 1, 1)
        _IntersectPower ("Intersect Power", Range(0.01, 100)) = 1
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
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _BaseColor;
            half4 _IntersectColor;
            float _IntersectPower;
            CBUFFER_END
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = ComputeScreenPos(o.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                const float2 screenUVs = i.uv.xy / i.uv.w;
                const float rawDepth = SampleSceneDepth(screenUVs);
                const float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                const float screenPosW = i.uv.w;
                float intersectAmount = sceneEyeDepth - screenPosW;
                intersectAmount = saturate(1.0f - intersectAmount);
                intersectAmount = pow(intersectAmount, _IntersectPower);

                return lerp(_BaseColor, _IntersectColor, intersectAmount);
            }
            ENDHLSL
        }
    }
}