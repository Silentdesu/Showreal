using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TechnoDemo.Code.Scripts.Shaders.GaussianBlur
{
    [System.Serializable, VolumeComponentMenu("QualityShaders/Gaussian Blur")]
    public sealed class GaussianBlurSettings : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("How large the convolution kernel is. A larger kernel mean stronger blurring.")]
        public ClampedIntParameter KernelSize = new(1, 1, 101);
        
        public bool IsActive() => KernelSize.value > 1 && active;
        public bool IsTileCompatible() => false;
    }
}