using System;
using Cinemachine;
using TechnoDemo.Core;
using TechnoDemo.Extensions;
using TechnoDemo.Input;
using TechnoDemo.Player;
using TechnoDemo.Spawn;
using UnityEngine;
using UnityEngine.Profiling;
using VContainer;

namespace TechnoDemo.Camera
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineFreeLook))]
    public sealed class CameraController : MonoBehaviour
    {
        private CinemachineFreeLook m_camera;
        private Transform m_target;
        private IGameManager m_gameManager;
        private IInput m_input;

        [Inject]
        private void Construct(IGameManager gameManager)
        {
            m_gameManager = gameManager;
        }

        private async void Start()
        {
            Profiler.BeginSample($"{nameof(CameraController)} SetTarget()");
            this.Log($"{nameof(CameraController)} Start() begin!");

            m_camera = GetComponent<CinemachineFreeLook>();
            if (m_gameManager.GetSpawnedContexts().TryGetObject(out IInput input))
            {
                m_input = input;
            }
            
            try
            {
                var opResult1 = await m_gameManager.GetSpawnedContexts().TryGetObjectAsync<ISpawner>();
                if (opResult1.Item1 == false) return;

                var opResult2 = await opResult1.Item2.GetSpawnedObjects().TryGetObjectAsync<IPlayer>();
                if (opResult2.Item1 == false) return;

                SetTarget(opResult2.Item2);
            }
            catch (Exception e)
            {
                this.LogError(e);
            }

            this.Log($"{nameof(CameraController)} Start() end!");
            Profiler.EndSample();
        }

        private void Update()
        {
            Profiler.BeginSample($"{nameof(CameraController)} Update()");
            m_camera.m_XAxis.m_InputAxisValue = m_input.Look.x;
            m_camera.m_YAxis.m_InputAxisValue = m_input.Look.y;
            Profiler.EndSample();
        }

        private void SetTarget(in IPlayer newTarget)
        {
            Profiler.BeginSample($"{nameof(CameraController)} SetTarget()");
            m_target = newTarget.Transform;

            m_camera.LookAt = m_target;
            m_camera.Follow = m_target;
            Profiler.EndSample();
        }
    }
}