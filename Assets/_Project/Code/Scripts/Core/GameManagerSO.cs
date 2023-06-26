using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TechnoDemo.Core
{
    [CreateAssetMenu(fileName = nameof(GameManagerSO), menuName = "SO/GameManager/Data")]
    public sealed class GameManagerSO : ScriptableObject
    {
        public SceneContext[] SceneContexts;
        public AssetReferenceGameObject PlayerRef;
    }
}