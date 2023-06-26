using UnityEngine;

namespace TechnoDemo.Player
{
    [CreateAssetMenu(fileName = nameof(PlayerSettingsSO), menuName = "SO/Player/Settings")]
    public sealed class PlayerSettingsSO : ScriptableObject
    {
        [Range(0.1f, 1000.0f)] public float SpeedMultiplier = 1.0f;
    }
}