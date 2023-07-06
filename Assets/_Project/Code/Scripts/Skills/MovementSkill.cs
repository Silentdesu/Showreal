using TechnoDemo.Extensions;
using TechnoDemo.Input;
using TechnoDemo.Interfaces;
using TechnoDemo.Player;
using UnityEngine;

namespace TechnoDemo.Skills
{
    public sealed class MovementSkill : Skill, ISetuper<MovementSkill>, IUpdateTickable
    {
        private CharacterController m_characterController;
        private Transform m_cameraTransform;
        private PlayerSettingsSO m_playerSettings;

        private float _currentSpeed;
        private float _animationBlend;

        private const float SPEED_CHANGE_RATE = 10.0f;
        
        public MovementSkill(ISkillHandler handler) : base(handler)
        {
        }

        public MovementSkill Setup(in IPlayer player)
        {
            m_characterController = player.CharacterController;
            m_cameraTransform = player.Camera.transform;
            return this;
        }

        public void UpdateTick(in IInput input)
        {
            if (!IsRunning()) return;

            var targetSpeed = input.IsSprinting ? m_playerSettings.SprintSpeed : m_playerSettings.RegularSpeed;

            if (input.Move == Vector2.zero) targetSpeed = 0.0f;

            var currentHorizontalSpeed =
                new Vector3(m_characterController.velocity.x, 0.0f, m_characterController.velocity.z).sqrMagnitude;
            currentHorizontalSpeed *= currentHorizontalSpeed;

            if (currentHorizontalSpeed < targetSpeed || currentHorizontalSpeed > targetSpeed)
            {
                _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * 1.0f, Time.deltaTime * SPEED_CHANGE_RATE);
                _currentSpeed = Mathf.Round(_currentSpeed * 1000.0f) / 1000.0f;
            }
            else
            {
                _currentSpeed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SPEED_CHANGE_RATE);
            if (_animationBlend < 0.01f) _animationBlend = 0.0f;

            var inputDir = new Vector3(input.Move.x, 0.0f, input.Move.y).normalized;
            
            Vector3 movement = m_cameraTransform.forward * input.Move + m_cameraTransform.right * input.Move;
            movement = movement.X0Y();
            m_characterController.Move(movement);
        }
    }
}