using UnityEngine;

namespace TechnoDemo.Player
{
    [CreateAssetMenu(fileName = nameof(PlayerSettingsSO), menuName = "SO/Player/Settings")]
    public sealed class PlayerSettingsSO : ScriptableObject
    {
        [SerializeField, Range(0.1f, 1000.0f)] private float m_sprintSpeed = 1.0f;
        [SerializeField, Range(0.1f, 1000.0f)] private float m_regularSpeed = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float m_rotationSmoothTime = 0.12f;

        [Header("Ground settings")] 
        [SerializeField] private float m_gravity = -9.81f;
        [SerializeField, Range(-1.0f, 1.0f)] private float m_groundOffset = -0.14f;
        [SerializeField, Range(0.0f, 1.5f)] private float m_groundSphereCastRadius = 0.28f;
        [SerializeField] private LayerMask m_groundLayer;

        [Header("Jump settings")] 
        [SerializeField] private float m_jumpHeight;
        [SerializeField, Range(0.0f, 0.15f)] private float m_fallTimeout = 0.15f; 
        
        public float SprintSpeed => m_sprintSpeed;
        public float RegularSpeed => m_regularSpeed;
        public float RotationSmoothTime => m_rotationSmoothTime;
        public float Gravit => m_gravity;
        public float GroundOffset => m_groundOffset;
        public float GroundSphereCastRadius => m_groundSphereCastRadius;
        public LayerMask GroundLayer => m_groundLayer;
        public float JumpHeight => m_jumpHeight;
        public float FallTimeout => m_fallTimeout;
    }
}