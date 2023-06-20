using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TechnoDemo.Spawn
{
    [CreateAssetMenu(fileName = nameof(SpawnerDataSO), menuName = "SO/Spawner/Data")]
    public sealed class SpawnerDataSO : ScriptableObject
    {
        public AssetReferenceGameObject PlayerRef;
    }
}