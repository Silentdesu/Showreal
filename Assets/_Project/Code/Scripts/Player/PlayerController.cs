using System;
using TechnoDemo.Core;
using TechnoDemo.Extensions;
using TechnoDemo.Input;
using UnityEngine;
using UnityEngine.Profiling;

namespace TechnoDemo.Player
{
    public interface IPlayer
    {
        Transform Transform { get; }
        void Inject(in IGameManager gameManager);
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour, IPlayer
    {
        public Transform Transform { get; private set; }

        private CharacterController m_controller;
        private IInput m_input;

        private PlayerSettingsSO m_settings;
        
        private void Awake()
        {
            Transform = transform;
        }

        private async void Start()
        {
            try
            {
                m_controller = GetComponent<CharacterController>();
            }
            catch (Exception e)
            {
                this.LogError(e);
            }
        }

        private void OnDestroy()
        {
            Transform = null;
        }

        private void Update()
        {
            Profiler.BeginSample($"{nameof(PlayerController)} Update()");
            
            Vector3 movement = m_input.Move;
            movement = movement.X0Y();
            m_controller.Move(movement * m_settings.SpeedMultiplier * Time.deltaTime);
            
            Profiler.EndSample();
        }

        void IPlayer.Inject(in IGameManager gameManager)
        {
            Profiler.BeginSample($"{nameof(PlayerController)} Inject()");
            
            m_settings = gameManager.ContainerSO.PlayerSettingsSo;

            if (gameManager.GetSpawnedContexts().TryGetObject(out IInput input))
                m_input = input;
            
            this.LogInjectSuccess();
            Profiler.EndSample();
        }
    }
}