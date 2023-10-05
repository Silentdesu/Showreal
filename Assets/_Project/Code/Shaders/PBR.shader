Shader "QualityShaders/PBR"
{
    Properties
    {
        [MainColor] _Color ("Color", Color) = (1, 1, 1, 1)
        [MainTex] _MainTex ("Texture", 2D) = "white" {}

        _MetallicTex ("Metallic Texture", 2D) = "white" {}
        _MetallicStrength ("Metallic Strength", Range(0, 1)) = 0

        _Smoothness ("Smoothness", Range(0, 1)) = 0

        _NormalTex ("Normal Texture", 2D) = "white" {}
        _NormalStrength ("Normal Strength", Range(0, 1)) = 0.5

        [Toggle(USE_EMISSION_ON)] _EnableEmission("Enable Emission", Float) = 0
        _EmissionTex ("Emission Texture", 2D) = "white" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)

        _AOTex ("Ambient Occlusion Texture", 2D) = "white" {}
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
            Name "PBR URP"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local USE_EMISSION_ON __

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 staticLightmapUV : TEXCOORD1;
                float2 dynamicLightmapUV : TEXCOORD2;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCORD2;
                float4 tangentWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                float4 shadowCoord : TEXCOORD5;
                DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 6);
                #ifdef DYNAMICLIGHTMAP_ON
                float2 dynamicLightmapUV : TEXCOORD7;
                #endif
            };

            sampler2D _MainTex;
            sampler2D _MetallicTex;
            sampler2D _NormalTex;
            sampler2D _EmissionTex;
            sampler2D _AOTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half4 _EmissionColor;
                float _MetallicStrength;
                float _Smoothness;
                float _NormalStrength;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;

                const VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
                const VertexNormalInputs normalInput = GetVertexNormalInputs(v.normalOS, v.tangentOS);
                
                o.positionWS = vertexInput.positionWS;
                o.positionCS = vertexInput.positionCS;
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.normalWS = normalInput.normalWS;
                
                float sign = v.tangentOS.w;
                o.tangentWS = float4(normalInput.tangentWS.xyz, sign);
                
                o.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                
                o.shadowCoord = GetShadowCoord(vertexInput);
                
                OUTPUT_LIGHTMAP_UV(v.staticLightmapUV, unity_LightmapST, o.staticLightmapUV);
                #ifdef DYNAMICLIGHTMAP_ON
                v.dynamicLightmapUV = v.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                OUTPUT_SH(o.normalWS.xyz, o.vertexSH);
                
                return o;
            }

            SurfaceData createSurfaceData(v2f i)
            {
                SurfaceData surfaceData = (SurfaceData)0;

                const float4 albedo = tex2D(_MainTex, i.uv);
                surfaceData.albedo = albedo.rgb * _Color.rgb;

                const float4 metallic = tex2D(_MetallicTex, i.uv);
                surfaceData.metallic = metallic * _MetallicStrength;

                surfaceData.smoothness = _Smoothness;

                float3 normal = UnpackNormal(tex2D(_NormalTex, i.uv));
                normal.rg *= _NormalStrength;
                surfaceData.normalTS = normal;

                // #if USE_EMISSION_ON
                // surfaceData.emission = tex2D(_EmissionTex, i.uv) * _EmissionColor;
                // #endif

                const float4 ao = tex2D(_AOTex, i.uv);
                surfaceData.occlusion = ao.r;

                surfaceData.alpha = albedo.a * _Color.a;

                return surfaceData;
            }

            InputData createInputData(v2f i, float3 normalTS)
            {
                InputData inputData = (InputData)0;

                inputData.positionWS = i.positionWS;
                
                const float3 bitagent = i.tangentWS.w * cross(i.normalWS, i.tangentWS.xyz);
                inputData.tangentToWorld = float3x3(i.tangentWS.xyz, bitagent, i.normalWS);
                inputData.normalWS = TransformTangentToWorld(normalTS, inputData.tangentToWorld);
                inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                
                inputData.viewDirectionWS = SafeNormalize(i.viewDirWS);
                
                inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                
                #if defined(DYNAMICLIGHTMAP_ON)
                inputData.bakedGI = SAMPLE_GI(i.staticLightmapUV, i.dynamicLightmapUV, i.vertexSH, inputData.normalWS);
                #else
                inputData.bakedGI = SAMPLE_GI(i.staticLightmapUV, i.vertexSH, inputData.normalWS);
                #endif
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
                inputData.shadowMask = SAMPLE_SHADOWMASK(i.staticLightmapUV);

                return inputData;
            }

            half4 frag(v2f i) : SV_Target
            {
                const SurfaceData surfaceData = createSurfaceData(i);
                const InputData inputData = createInputData(i, surfaceData.normalTS);

                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Lit"
}