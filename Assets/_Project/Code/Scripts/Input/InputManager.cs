using System;
using MessagePipe;
using TechnoDemo.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace TechnoDemo.Input
{
    public interface IInput
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
    }
    
    [DisallowMultipleComponent]
    public sealed class InputManager : SceneContext, IInput
    {
        public Vector2 Move => m_gameInput.Main.Move.ReadValue<Vector2>();
        public Vector2 Look => m_gameInput.Main.Look.ReadValue<Vector2>();
        
        private GameInputMap m_gameInput;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
        }
        
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