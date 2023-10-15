using UnityEngine.Rendering.Universal;

namespace TechnoDemo.Code.Scripts.Shaders
{
    public sealed class GrayscaleFeature : ScriptableRendererFeature
    {
        private GrayscaleRenderPass m_renderPass;
        
        public override void Create()
        {
            name = "Grayscale";
            m_renderPass = new GrayscaleRenderPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_renderPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            m_renderPass.Setup("Grayscale Post Process", renderer.cameraColorTargetHandle);
        }
    }
}