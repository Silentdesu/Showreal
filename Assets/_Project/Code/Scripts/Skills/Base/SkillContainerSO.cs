using UnityEngine;

namespace TechnoDemo.Skills
{
    [CreateAssetMenu(fileName = nameof(SkillContainerSO), menuName = "SO/Skill/Container")]
    public sealed class SkillContainerSO : ScriptableObject
    {
        [field: SerializeField] public SkillData[] Data { get; private set; }
    }
}