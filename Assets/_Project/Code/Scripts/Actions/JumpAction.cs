using System;
using MessagePipe;
using TechnoDemo.Input;
using TechnoDemo.Interfaces;
using TechnoDemo.Player;
using UnityEngine;

namespace TechnoDemo.Actions
{
    public readonly struct JumpMessage : IEquatable<JumpMessage>
    {
        public readonly float VerticalVelocity;

        public JumpMessage(float verticalVelocity)
        {
            VerticalVelocity = verticalVelocity;
        }

        public bool Equals(JumpMessage other)
        {
            return VerticalVelocity.Equals(other.VerticalVelocity);
        }

        public override bool Equals(object obj)
        {
            return obj is JumpMessage other && Equals(other);
        }

        public override int GetHashCode()
        {
            return VerticalVelocity.GetHashCode();
        }
    }

    public sealed class JumpAction : BaseAction, ISetuper<JumpAction>, IUpdateTickable, IDisposable
    {
        private CharacterController m_characterController;
        private Animator m_animator;
        private PlayerSettingsSO m_playerSettings;

        private float m_verticalVelocity;
        private float m_fallTimeout;
        private float m_jumpTimeoutDelta;

        private bool m_grounded;

        private IPublisher<JumpMessage> m_publisher;
        private IDisposable m_disposable;

        public JumpAction(IActionHandler handler, IPublisher<JumpMessage> publisher,
            ISubscriber<GroundMessage> groundSubscriber)
            : base(handler)
        {
            m_publisher = publisher;
            var disposableBag = DisposableBag.CreateBuilder();
            groundSubscriber.Subscribe(OnGroundMessageReceive).AddTo(disposableBag);
            m_disposable = disposableBag.Build();
        }

        public JumpAction Setup(in IPlayer player)
        {
            m_characterController = player.CharacterController;
            m_animator = player.Animator;
            m_playerSettings = player.Settings;

            return this;
        }

        public void UpdateTick(in IInput input)
        {
            JumpAndGravity(input);
        }

        private void JumpAndGravity(in IInput input)
        {
            if (!m_grounded) return;
            
            m_fallTimeout = m_playerSettings.FallTimeout;

            if (m_animator)
            {
                // m_animator.SetBool(AnimatorParameters.Jump, false);
                // m_animator.SetBool(AnimatorParameters.FreeFall, false);
            }

            if (m_verticalVelocity < 0.0f) m_verticalVelocity = -2.0f;

            if (input.IsJumping && m_jumpTimeoutDelta <= 0.0f)
            {
                m_verticalVelocity = Mathf.Sqrt(m_playerSettings.JumpHeight * -2.0f * m_playerSettings.Gravity);

                if (m_animator)
                {
                    m_animator.SetInteger(AnimatorParameters.ActionID, Data.Id);
                    // m_animator.SetBool(AnimatorParameters.Jump, true);
                }
            }
            
            m_characterController.Move(Vector3.up * (m_verticalVelocity * Time.deltaTime));

            m_publisher?.Publish(new JumpMessage(m_verticalVelocity));
        }

        private void OnGroundMessageReceive(GroundMessage message)
        {
            m_grounded = message.Grounded;
        }

        void IDisposable.Dispose()
        {
            m_disposable?.Dispose();
        }
    }

    public static partial class AnimatorParameters
    {
        public static int Jump = Animator.StringToHash("Jump");
        public static int FreeFall = Animator.StringToHash("FreeFall");
    }
}