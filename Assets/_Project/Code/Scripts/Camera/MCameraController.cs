using System;
using Cinemachine;
using TechnoDemo.Player;
using TechnoDemo.Spawn;
using UnityEngine;
using VContainer;

namespace TechnoDemo.Camera
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ICinemachineCamera))]
    public sealed class MCameraController : MonoBehaviour
    {
        private ICinemachineCamera m_camera;
        private Transform m_target;
        private ISpawner m_spawner;
        
        [Inject]
        private void Construct(ISpawner spawner)
        {
            m_spawner = spawner;
        }

        private async void Start()
        {
            this.Log($"{nameof(MCameraController)} Start() begin!");
            
            m_camera = GetComponent<ICinemachineCamera>();
            
            var opResult = await m_spawner.TryGetObjectAsync<IPlayer>();

            if (opResult.Item1 == false) return;
            
            m_target = opResult.Item2.Transform;
            
            m_camera.LookAt = m_target;
            m_camera.Follow = m_target;
            
            this.Log($"{nameof(MCameraController)} Start() end!");
        }
    }
}