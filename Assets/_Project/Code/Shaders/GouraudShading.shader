Shader "QualityShaders/GouraudShading"
{
    Properties
    {
        [MainColor] _Color ("Color", Color) = (1, 1, 1, 1)
        [MainTex] _MainTex ("Texture", 2D) = "white" {}
        
        _GlossPower ("Gloss Power", Float) = 400
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
            Name "GouraudShading"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float4 diffuseLighting : TEXCOORD1;
                float4 specularLighting : TEXCOORD2;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half _GlossPower;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                const float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                const float3 positionWS = mul(unity_ObjectToWorld, v.positionOS);
                const float3 viewWS = GetWorldSpaceNormalizeViewDir(positionWS);

                const float3 ambient = SampleSHVertex(normalWS);
                const Light mainLight = GetMainLight();
                const float3 diffuse = mainLight.color * max(0, dot(normalWS, mainLight.direction));
                const float3 halfVector = normalize(mainLight.direction + viewWS);
                float specular = max(0, dot(normalWS, halfVector));
                specular = pow(specular, _GlossPower);
                const float3 specularColor = mainLight.color * specular;

                o.diffuseLighting = float4(ambient + diffuse, 1.0f);
                o.specularLighting = float4(specularColor, 1.0f);
                
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                const half4 texSample = tex2D(_MainTex, i.uv);
                return texSample * _Color * i.diffuseLighting + i.specularLighting;
            }
            ENDHLSL
        }
    }
}