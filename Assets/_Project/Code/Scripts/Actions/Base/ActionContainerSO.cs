using UnityEngine;

namespace TechnoDemo.Actions
{
    [CreateAssetMenu(fileName = nameof(ActionContainerSO), menuName = "SO/Skill/Container")]
    public sealed class ActionContainerSO : ScriptableObject
    {
        [field: SerializeField] public ActionData[] Data { get; private set; }
    }
}