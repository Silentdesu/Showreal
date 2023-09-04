Shader "QualityShaders/StencilMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [IntRange] _StencilRef("Stencil ref", Range(0, 255)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry-1"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Stencil
            {
                Ref[_StencilRef]    
                Comp Always
                Pass Replace
            }
            
            ZWrite Off
        }
    }
    
    Fallback Off
}