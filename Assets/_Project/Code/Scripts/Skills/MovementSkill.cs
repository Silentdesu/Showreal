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
            
            Vector3 movement = m_cameraTransform.forward * input.Move + m_cameraTransform.right * input.Move;
            movement = movement.X0Y();
            m_characterController.Move(movement);
        }
    }
}