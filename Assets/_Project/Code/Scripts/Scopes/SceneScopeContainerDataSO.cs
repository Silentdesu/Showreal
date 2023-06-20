using TechnoDemo.Spawn;
using UnityEngine;

namespace TechnoDemo.Scopes
{
    [CreateAssetMenu(fileName = nameof(SceneScopeContainerDataSO), menuName = "SO/Scopes/Scene Container")]
    public sealed class SceneScopeContainerDataSO : ScriptableObject
    {
        public SpawnerDataSO SpawnerDataSo;
    }
}