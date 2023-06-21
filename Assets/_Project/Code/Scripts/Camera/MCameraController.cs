using Cinemachine;
using TechnoDemo.Core;
using TechnoDemo.Extensions;
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
        private IGameManager m_gameManager;
        
        [Inject]
        private void Construct(IGameManager gameManager)
        {
            m_gameManager = gameManager;
        }

        private async void Start()
        {
            this.Log($"{nameof(MCameraController)} Start() begin!");
            
            m_camera = GetComponent<ICinemachineCamera>();

            var opResult1 = await m_gameManager.GetSpawnedContexts().TryGetObjectAsync<ISpawner>();

            if (opResult1.Item1 == false) return;
            
            var opResult2 = await opResult1.Item2.GetSpawnedObjects().TryGetObjectAsync<IPlayer>(); 

            if (opResult2.Item1 == false) return;
            
            m_target = opResult2.Item2.Transform;
            
            m_camera.LookAt = m_target;
            m_camera.Follow = m_target;
            
            this.Log($"{nameof(MCameraController)} Start() end!");
        }
    }
}