using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TechnoDemo.Code.Scripts.Shaders
{
    public sealed class GrayscaleRenderPass : ScriptableRenderPass
    {
        private Material m_material;
        private GrayscaleSettings m_settings;
        private RenderTargetIdentifier m_source;
        private RenderTargetIdentifier m_mainTex;
        private string m_profilerTag;
        private static readonly int Strength = Shader.PropertyToID("_Strength");

        public void Setup(string profilerTag, RenderTargetIdentifier renderTargetIdentifier)
        {
            m_profilerTag = profilerTag;
            m_source = renderTargetIdentifier;
            var stack = VolumeManager.instance.stack;
            m_settings = stack.GetComponent<GrayscaleSettings>();
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

            if (m_settings && m_settings.IsActive())
            {
                m_material = new Material(Shader.Find("QualityShaders/Grayscale"));
            }
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (!m_settings) return;

            var id = Shader.PropertyToID("_MainTex");
            m_mainTex = new RenderTargetIdentifier(id);
            cmd.GetTemporaryRT(id, renderingData.cameraData.cameraTargetDescriptor);
            base.OnCameraSetup(cmd, ref renderingData);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!m_settings.IsActive()) return;

            var cmd = CommandBufferPool.Get(m_profilerTag);
            cmd.Blit(m_source, m_mainTex);
            m_material.SetFloat(Strength, m_settings.Strength.value);
            cmd.Blit(m_mainTex, m_source, m_material);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(Shader.PropertyToID("_MainTex"));
        }
    }
}