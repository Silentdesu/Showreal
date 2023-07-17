using System;
using MessagePipe;
using TechnoDemo.Input;
using TechnoDemo.Interfaces;
using TechnoDemo.Player;
using UnityEngine;

namespace TechnoDemo.Actions
{
    public sealed class MovementAction : BaseAction, ISetuper<MovementAction>, IUpdateTickable, IDisposable
    {
        private Transform m_transform;
        private Transform m_cameraTransform;
        private Animator m_animator;
        private CharacterController m_characterController;
        private PlayerSettingsSO m_playerSettings;

        private IDisposable m_disposable;
        
        private float m_currentSpeed;
        private float m_verticalVelocity;
        private float m_animationBlend;
        private float m_targetRotation;
        private float m_rotationVelocity;

        private const float SPEED_CHANGE_RATE = 10.0f;

        public MovementAction(IActionHandler handler, ISubscriber<JumpMessage> jumpSubscriber) : base(handler)
        {
            var disposableBag = DisposableBag.CreateBuilder();
            jumpSubscriber.Subscribe(OnJumpMessageReceive).AddTo(disposableBag);
            m_disposable = disposableBag.Build();
        }

        public MovementAction Setup(in IPlayer player)
        {
            m_transform = player.Transform;
            m_animator = player.Animator;
            m_characterController = player.CharacterController;
            m_cameraTransform = player.Camera.transform;
            m_playerSettings = player.Settings;

            return this;
        }

        public void UpdateTick(in IInput input)
        {
            Move(input);
        }

        private void Move(in IInput input)
        {
            var targetSpeed = input.IsSprinting ? m_playerSettings.SprintSpeed : m_playerSettings.RegularSpeed;

            if (input.Move == Vector2.zero) targetSpeed = 0.0f;

            var characterVelocity = m_characterController.velocity;
            var currentHorizontalSpeed = new Vector3(characterVelocity.x, 0.0f, characterVelocity.z).magnitude;
            var inputMove = input.Move.magnitude;
            var tSpeed = targetSpeed * targetSpeed;
            const float speedOffset = 0.1f;

            if (currentHorizontalSpeed < tSpeed - speedOffset || currentHorizontalSpeed > tSpeed + speedOffset)
            {
                m_currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMove,
                    Time.deltaTime * SPEED_CHANGE_RATE);
                m_currentSpeed = Mathf.Round(m_currentSpeed * 1000.0f) / 1000.0f;
            }
            else
            {
                m_currentSpeed = targetSpeed;
            }

            m_animationBlend = Mathf.Lerp(m_animationBlend, targetSpeed, Time.deltaTime * SPEED_CHANGE_RATE);
            if (m_animationBlend < 0.01f) m_animationBlend = 0.0f;

            if (input.Move != Vector2.zero)
            {
                m_targetRotation = Mathf.Atan2(input.Move.x, input.Move.y) * Mathf.Rad2Deg + m_cameraTransform.eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(m_transform.eulerAngles.y, m_targetRotation, ref m_rotationVelocity, m_playerSettings.RotationSmoothTime);
                m_transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            var targetDirection = Quaternion.Euler(0.0f, m_targetRotation, 0.0f) * Vector3.forward;

            m_characterController.Move(targetDirection * (m_currentSpeed * Time.deltaTime) +
                                       Vector3.up * (m_verticalVelocity * Time.deltaTime));

            if (m_animator)
            {
                m_animator.SetFloat(AnimatorParameters.Speed, m_animationBlend);
                m_animator.SetFloat(AnimatorParameters.MotionSpeed, inputMove);
            }
        }

        private void OnJumpMessageReceive(JumpMessage message)
        {
            m_verticalVelocity = message.VerticalVelocity;
        }

        public void Dispose()
        {
            m_disposable?.Dispose();
        }
    }

    public static partial class AnimatorParameters
    {
        public static int Speed = Animator.StringToHash("Speed");
        public static int Grounded = Animator.StringToHash("Grounded");
        public static int MotionSpeed = Animator.StringToHash("MotionSpeed");
    }
}