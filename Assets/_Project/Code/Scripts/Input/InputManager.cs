using TechnoDemo.Core;
using UnityEngine;

namespace TechnoDemo.Input
{
    public interface IInput
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool IsSprinting { get; }
        bool IsJumping { get; }
    }
    
    [DisallowMultipleComponent]
    public sealed class InputManager : SceneContext, IInput
    {
        public Vector2 Move => m_gameInput.Main.Move.ReadValue<Vector2>();
        public Vector2 Look => m_gameInput.Main.Look.ReadValue<Vector2>();
        public bool IsSprinting => m_gameInput.Main.Sprint.ReadValue<float>() > 0.0f;
        public bool IsJumping => m_gameInput.Main.Jump.triggered;
        
        private GameInputMap m_gameInput;
        
        private void Awake()
        {
            m_gameInput = new GameInputMap();
        }

        private void OnEnable()
        {
            m_gameInput.Enable();
        }

        private void OnDisable()
        {
            m_gameInput.Disable();
        }
    }
}