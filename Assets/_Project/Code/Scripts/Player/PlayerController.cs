using System;
using System.Collections;
using System.Collections.Generic;
using TechnoDemo.Core;
using TechnoDemo.Extensions;
using TechnoDemo.Input;
using TechnoDemo.Interfaces;
using TechnoDemo.Skills;
using UnityEngine;
using UnityEngine.Profiling;
using VContainer;

namespace TechnoDemo.Player
{
    public interface IPlayer
    {
        Transform Transform { get; }
        CharacterController CharacterController { get; }
        Animator Animator { get; }
        UnityEngine.Camera Camera { get; }
        PlayerSettingsSO Settings { get; }
        
        void Inject(in IGameManager gameManager);
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour, IPlayer
    {
        public Transform Transform { get; private set; }
        public CharacterController CharacterController { get; private set; }
        public Animator Animator { get; private set; }
        public UnityEngine.Camera Camera { get; private set; }
        public PlayerSettingsSO Settings { get; private set; }

        private IInput m_input;
        private ISkillHandler m_skillHandler;

        [Inject]
        private void Construct(ISkillHandler skillHandler)
        {
            m_skillHandler = skillHandler;
        }

        private IEnumerator Start()
        {
            Transform = transform;
            Camera = UnityEngine.Camera.main;
            CharacterController = GetComponent<CharacterController>();
            
            m_skillHandler.AddSkill(new MovementSkill(m_skillHandler).Setup(this));
            
            yield return null;
        }

        private void OnDestroy()
        {
            Transform = null;
        }

        private void Update()
        {
            Span<IUpdateTickable> skills = m_skillHandler.UpdateTickSkills;

            for (int i = 0, count = skills.Length; i < count; i++) skills[i].UpdateTick(m_input);
        }

        void IPlayer.Inject(in IGameManager gameManager)
        {
            Profiler.BeginSample($"{nameof(PlayerController)} Inject()");

            Settings = gameManager.ContainerSO.PlayerSettingsSo;

            if (gameManager.GetSpawnedContexts().TryGetObject(out IInput input))
                m_input = input;

            this.LogInjectSuccess();
            Profiler.EndSample();
        }
    }
}