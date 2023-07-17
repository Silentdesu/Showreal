using System;
using System.Collections;
using MessagePipe;
using TechnoDemo.Actions;
using TechnoDemo.Core;
using TechnoDemo.Extensions;
using TechnoDemo.Input;
using TechnoDemo.Interfaces;
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

        private IObjectResolver m_resolver;
        private IInput m_input;
        private IActionHandler m_actionHandler;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            m_resolver = resolver;
            m_actionHandler = resolver.Resolve<IActionHandler>();
        }

        void IPlayer.Inject(in IGameManager gameManager)
        {
            Profiler.BeginSample($"{nameof(PlayerController)} {nameof(IPlayer.Inject)}()");
            Settings = gameManager.ContainerSO.PlayerSettingsSo;

            if (gameManager.GetSpawnedContexts().TryGetObject(out IInput input))
                m_input = input;

            this.LogInjectSuccess();
            Profiler.EndSample();
        }

        private IEnumerator Start()
        {
            Transform = transform;
            Camera = UnityEngine.Camera.main;
            Animator = GetComponentInChildren<Animator>();
            CharacterController = GetComponent<CharacterController>();

            m_actionHandler.AddSkill(new MovementAction(m_actionHandler, m_resolver
                .Resolve<ISubscriber<JumpMessage>>()).Setup(this));
            m_actionHandler.AddSkill(new JumpAction(m_actionHandler, m_resolver
                .Resolve<IPublisher<JumpMessage>>(), m_resolver.Resolve<ISubscriber<GroundMessage>>()).Setup(this));
            m_actionHandler.AddSkill(new GroundAction(m_actionHandler, m_resolver
                .Resolve<IPublisher<GroundMessage>>()).Setup(this));
            yield return null;
        }

        private void OnDestroy()
        {
            Transform = null;
        }

        private void Update()
        {
            Profiler.BeginSample($"{nameof(PlayerController)} {nameof(Update)}()");
            Span<IUpdateTickable> actions = m_actionHandler.UpdateTickActions;

            for (int i = 0, count = actions.Length; i < count; i++)
            {
                if (actions[i] is IAction action && action.IsRunning())
                    actions[i].UpdateTick(m_input);
            }

            Profiler.EndSample();
        }

        public void OnDrawGizmos()
        {
            Profiler.BeginSample($"{nameof(PlayerController)} {nameof(OnDrawGizmos)}");
            Span<IUpdateTickable> actions = m_actionHandler.UpdateTickActions;

            for (int i = 0, count = actions.Length; i < count; i++)
            {
                if (actions[i] is IDev dev) dev.OnDrawGizmos();
            }

            Profiler.EndSample();
        }
    }
}