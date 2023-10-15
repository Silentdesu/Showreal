using UnityEngine.Rendering.Universal;

namespace TechnoDemo.Code.Scripts.Shaders.GaussianBlur
{
    public sealed class GaussianBlurFeature : ScriptableRendererFeature
    {
        private GaussianBlurRenderPass m_renderPass;
        
        public override void Create()
        {
            name = "Gaussian Blur";
            m_renderPass = new GaussianBlurRenderPass();
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            m_renderPass.Setup("Gaussian Blur Post Process", renderer.cameraColorTargetHandle);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_renderPass);
        }
    }
}