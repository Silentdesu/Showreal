Shader "QualityShaders/Wave"
{
    Properties
    {
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseTex ("Texture", 2D) = "white" {}
        
        _WaveStrength ("Wave Strength", Range(0, 2)) = 0.1
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1

        [Enum(UnityEngine.Rendering.BlendMode)]
        _SrcBlend ("Source Blend Factor", Int) = 1

        [Enum(UnityEngine.Rendering.BlendMode)]
        _DstBlend ("Destination Blend Factor", Int) = 1
        
        _TessAmount ("Subdivision", Range(1, 64)) = 2
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 300

        Pass
        {
            Blend [_SrcBlend][_DstBlend]

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma taget 4.6
            #pragma vertex vert
            #pragma hull tessHull
            #pragma domain tessDomain
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct tessControlPoint
            {
                float4 positionOS : INTERNALTESSPOS;
                float2 uv : TEXCOORD0;
            };

            struct tessFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _BaseTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseTex_ST;
                half4 _BaseColor;
                half _WaveStrength;
                half _WaveSpeed;
                half _TessAmount;
            CBUFFER_END

            tessControlPoint vert(const appdata v)
            {
                tessControlPoint output;

                output.positionOS = v.positionOS;
                output.uv = v.uv;

                return output;
            }

            v2f tessVert(appdata v)
            {
                v2f o;

                float4 positionWS = mul(unity_ObjectToWorld, v.positionOS);
                float height = sin(_Time.y * _WaveSpeed + positionWS.x + positionWS.z);
                positionWS.y += height * _WaveStrength;

                o.positionCS = mul(UNITY_MATRIX_VP, positionWS);
                o.uv = TRANSFORM_TEX(v.uv, _BaseTex);

                return o;
            }

            tessFactors patchConstantFunc(InputPatch<tessControlPoint, 3> patch)
            {
                tessFactors f;

                f.edge[0] = f.edge[1] = f.edge[2] = _TessAmount;
                f.inside = _TessAmount;

                return f;
            }

            [domain("tri")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [partitioning("fractional_even")]
            [patchconstantfunc("patchConstantFunc")]
            tessControlPoint tessHull(InputPatch<tessControlPoint, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            [domain("tri")]
            v2f tessDomain(tessFactors factors, OutputPatch<tessControlPoint, 3> patch, float3 bcCoords : SV_DomainLocation)
            {
                appdata v;

                v.positionOS =
                    patch[0].positionOS * bcCoords.x +
                    patch[1].positionOS * bcCoords.y + patch[2].positionOS * bcCoords.z;

                v.uv = patch[0].uv * bcCoords.x + patch[1].uv * bcCoords.y + patch[2].uv * bcCoords.z;

                return tessVert(v);
            }

            half4 frag(v2f i) : SV_Target
            {
                const half3 baseMapSample = tex2D(_BaseTex, i.uv).rgb;
                return half4(baseMapSample, 1.0f) * _BaseColor;
            }
            ENDHLSL
        }
    }
    Fallback Off
}