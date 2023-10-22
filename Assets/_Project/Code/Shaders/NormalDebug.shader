Shader "Unlit/NormalDebug"
{
    Properties
    {
        [MainColor] _DebugColor ("Debug Color", Color) = (1, 0, 0, 1)

        _WireThickness ("Wire Thickness", Range(0, 0.1)) = 0.01
        _WireLength ("Wire Length", Range(0, 1)) = 0.2
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

        Pass
        {
            Name "NormalDebug"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct v2g
            {
                float4 positionWS : SV_POSITION;
                float3 normalWS : NORMAL;
                float4 tangentWS : TENGENT;
            };

            struct g2f
            {
                float4 positionCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                half3 _DebugColor;
                half _WireThickness;
                half _WireLength;
            CBUFFER_END

            v2g vert(appdata i)
            {
                v2g o;
                o.positionWS = mul(unity_ObjectToWorld, i.positionOS);
                o.normalWS = TransformObjectToWorldNormal(i.normalOS);
                o.tangentWS = mul(unity_ObjectToWorld, i.tangentOS);

                return o;
            }

            g2f geomToClip(float3 positionOS, float3 offsetOS)
            {
                g2f o;
                o.positionCS = mul(UNITY_MATRIX_VP, half4(positionOS + offsetOS, 1.0f));
                return o;
            }

            [maxvertexcount(8)]
            void geom(point v2g i[1], inout TriangleStream<g2f> triStream)
            {
                const float3 normal = normalize(i[0].normalWS);
                float4 tangent = normalize(i[0].tangentWS);
                const float3 bitagent = normalize(cross(normal, tangent.xyz) * tangent.w);

                const float3 xOffset = tangent.xyz * _WireThickness * 0.5f;
                const float3 yOffset = normal * _WireLength;
                const float3 zOffset = bitagent * _WireThickness * 0.5f;

                const float3 offsets[8] =
                {
                    -xOffset,
                    xOffset,
                    -xOffset + yOffset,
                    xOffset + yOffset,

                    -zOffset,
                    zOffset,
                    -zOffset + yOffset,
                    zOffset + yOffset
                };

                const float3 pos = i[0].positionWS.xyz;

                triStream.Append(geomToClip(pos, offsets[0]));
                triStream.Append(geomToClip(pos, offsets[1]));
                triStream.Append(geomToClip(pos, offsets[2]));
                triStream.Append(geomToClip(pos, offsets[3]));

                triStream.RestartStrip();

                triStream.Append(geomToClip(pos, offsets[4]));
                triStream.Append(geomToClip(pos, offsets[5]));
                triStream.Append(geomToClip(pos, offsets[6]));
                triStream.Append(geomToClip(pos, offsets[7]));

                triStream.RestartStrip();
            }

            half4 frag(g2f i) : SV_Target
            {
                return half4(_DebugColor, 1.0);
            }
            ENDHLSL
        }
    }
    Fallback Off
}