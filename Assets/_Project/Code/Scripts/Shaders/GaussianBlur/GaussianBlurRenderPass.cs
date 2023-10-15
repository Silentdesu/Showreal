using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TechnoDemo.Code.Scripts.Shaders.GaussianBlur
{
    public sealed class GaussianBlurRenderPass : ScriptableRenderPass
    {
        private Material m_material;
        private GaussianBlurSettings m_settings;
        private RenderTargetIdentifier m_sourceTex;
        private RenderTargetIdentifier m_mainTex;
        private RenderTargetIdentifier m_tempTex;
        private string m_profilerTag;
        private static readonly int KernelSize = Shader.PropertyToID("_KernelSize");

        private const string SHADER_NAME = "QualityShaders/GaussianBlur";
        private const string MAIN_TEX = "_MainTex";
        private const string TEMP_TEX = "_TempTex";
        
        public void Setup(string profilerTag, RenderTargetIdentifier source)
        {
            m_profilerTag = profilerTag;
            m_sourceTex = source;
            var stack = VolumeManager.instance.stack;
            m_settings = stack.GetComponent<GaussianBlurSettings>();
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

            if (m_settings && m_settings.IsActive())
            {
                m_material = new Material(Shader.Find(SHADER_NAME));
            }
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (!m_settings) return;

            var id = Shader.PropertyToID(MAIN_TEX);
            m_mainTex = new RenderTargetIdentifier(id);
            cmd.GetTemporaryRT(id, renderingData.cameraData.cameraTargetDescriptor);

            id = Shader.PropertyToID(TEMP_TEX);
            m_tempTex = new RenderTargetIdentifier(id);
            cmd.GetTemporaryRT(id, renderingData.cameraData.cameraTargetDescriptor);
            
            base.OnCameraSetup(cmd, ref renderingData);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!m_settings.IsActive()) return;

            var cmd = CommandBufferPool.Get(m_profilerTag);
            cmd.Blit(m_sourceTex, m_mainTex);
            
            m_material.SetInt(KernelSize, m_settings.KernelSize.value);
            
            cmd.Blit(m_mainTex, m_tempTex, m_material, 0);
            cmd.Blit(m_tempTex, m_sourceTex, m_material, 1);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(MAIN_TEX));
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(TEMP_TEX));
        }
    }
}