Shader "QualityShaders/PhongShading"
{
    Properties
    {
        [MainColor] _Color ("Color", Color) = (1, 1, 1, 1)
        [MainTex] _MainTex ("Texture", 2D) = "white" {}

        _GlossPower ("Gloss Power", Float) = 400
        _FresnelPower ("Fresnel Power", Float) = 5
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
            Name "Phong Shading"
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
                float3 normalWS : TEXCOORD1;
                float3 viewWS : TEXCOORD2;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half _GlossPower;
                half _FresnelPower;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);

                const float3 positionWS = mul(unity_ObjectToWorld, v.positionOS.xyz);
                o.viewWS = GetWorldSpaceViewDir(positionWS);
                
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                const float3 normal = normalize(i.normalWS);
                const float3 view = normalize(i.viewWS);

                const float3 ambient = SampleSH(i.normalWS);
                const Light mainLight = GetMainLight();

                const float3 diffuse = mainLight.color * max(0, dot(normal, mainLight.direction));
                const float3 halfVector = normalize(mainLight.direction + view);
                float specular = max(0, dot(normal, halfVector));
                specular = pow(specular, _GlossPower);
                const float3 specularColor = mainLight.color * specular;

                float fresnel = 1.0f - max(0, dot(normal, view));
                fresnel = pow(fresnel, _FresnelPower);
                const float3 fresnelColor = mainLight.color * fresnel;
                
                const float4 diffuseLighting = float4(ambient + diffuse, 1.0f);
                const float4 specularLighting = float4(specularColor + fresnelColor, 1.0f);

                const half4 texSample = tex2D(_MainTex, i.uv);
                return texSample * _Color * diffuseLighting + specularLighting;
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Simple Lit"
}