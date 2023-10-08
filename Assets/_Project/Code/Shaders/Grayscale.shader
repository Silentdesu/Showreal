Shader "QualityShaders/Grayscale"
{
    Properties
    {
        [MainTex] _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 300

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half _Strength;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                const half4 texSample = tex2D(_MainTex, i.uv);
                const half3 color = Luminance(texSample);
                return lerp(texSample, half4(color, 1.0f), _Strength);
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Lit"
}