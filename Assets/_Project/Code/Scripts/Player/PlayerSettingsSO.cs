using UnityEngine;

namespace TechnoDemo.Player
{
    [CreateAssetMenu(fileName = nameof(PlayerSettingsSO), menuName = "SO/Player/Settings")]
    public sealed class PlayerSettingsSO : ScriptableObject
    {
        [Range(0.1f, 1000.0f)] private float _sprintSpeed = 1.0f;
        [Range(0.1f, 1000.0f)] private float _regularSpeed = 1.0f;

        public float SprintSpeed => _sprintSpeed;
        public float RegularSpeed => _regularSpeed;
    }
}