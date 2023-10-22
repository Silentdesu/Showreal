Shader "Unlit/TessLOD"
{
    Properties
    {
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1)
        [MainTexutre] _BaseMap ("Texture", 2D) = "white" {}

        [Enum(UnityEngine.Rendering.BlendMode)]
        _SrcBlend ("Source Blend Factor", Int) = 1

        [Enum(UnityEngine.Rendering.BlendMode)]
        _DstBlend ("Destination Blend Factor", Int) = 1

        _TessMindDistance ("Tesselation Min Distance", Float) = 20
        _TessMaxDistance ("Tesselation Max Distance", Float) = 50
        _TessAmount ("Tesselation Amount", Range(1, 64)) = 2
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
            Name "TessLOD"
            Tags { "LightMode" = "UniversalForward"}
         
            Blend [_SrcBlend][_DstBlend]
               
            HLSLPROGRAM
            #pragma target 4.6
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma hull tessHull
            #pragma domain tessDomain

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct tessControlPoint
            {
                float4 positionWS : INTERNALTESSPOS;
                float2 uv : TEXCOORD0;
            };

            struct tessFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _TessAmount;
                half _TessMinDistance;
                half _TessMaxDistance;
            CBUFFER_END

            tessControlPoint vert(appdata v)
            {
                tessControlPoint o;
                o.positionWS = mul(unity_ObjectToWorld, v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            tessFactors patchConstantFunc(InputPatch<tessControlPoint, 3> patch)
            {
                tessFactors o;

                const float3 trisPos0 = patch[0].positionWS.xyz;
                const float3 trisPos1 = patch[1].positionWS.xyz;
                const float3 trisPos2 = patch[2].positionWS.xyz;

                const float3 edgePos0 = 0.5f * (trisPos1 + trisPos2);
                const float3 edgePos1 = 0.5f * (trisPos0 + trisPos2);
                const float3 edgePos2 = 0.5f * (trisPos0 + trisPos1);

                const float3 cameraPos = GetCameraPositionWS();

                const float3 dist0 = distance(edgePos0, cameraPos);
                const float3 dist1 = distance(edgePos1, cameraPos);
                const float3 dist2 = distance(edgePos2, cameraPos);

                const float fadeDist = _TessMaxDistance - _TessMinDistance;

                const float edgeFactor0 = saturate(1.0f - (dist0 - _TessMinDistance) / fadeDist);
                const float edgeFactor1 = saturate(1.0f - (dist1 - _TessMinDistance) / fadeDist);
                const float edgeFactor2 = saturate(1.0f - (dist2 - _TessMinDistance) / fadeDist);

                o.edge[0] = max(pow(edgeFactor0, 2) * _TessAmount, 1);
                o.edge[1] = max(pow(edgeFactor1, 2) * _TessAmount, 1);
                o.edge[2] = max(pow(edgeFactor2, 2) * _TessAmount, 1);

                o.inside = (o.edge[0] + o.edge[1] + o.edge[2]) / 3.0f;

                return o;
            }

            [domain("tri")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [partitioning("integer")]
            [patchconstantfunc("patchConstantFunc")]
            tessControlPoint tessHull(InputPatch<tessControlPoint, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            [domain("tri")]
            v2f tessDomain(tessFactors factors, OutputPatch<tessControlPoint, 3> patch, float3 bcCoords : SV_DomainLocation)
            {
                v2f o;

                float4 positionWS = patch[0].positionWS * bcCoords.x + patch[1].positionWS * bcCoords.y + patch[2].positionWS * bcCoords.z;
                o.positionCS = mul(UNITY_MATRIX_VP, positionWS);
                o.uv = patch[0].uv * bcCoords.x + patch[1].uv * bcCoords.y + patch[2].uv * bcCoords.z;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                const half3 baseMapSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).rgb;
                half3 outputColor = baseMapSample * _BaseColor.rgb;

                return half4(outputColor, 1.0);
            }
            ENDHLSL
        }
    }
}