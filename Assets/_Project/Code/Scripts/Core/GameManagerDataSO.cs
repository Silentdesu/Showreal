using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TechnoDemo.Core
{
    [CreateAssetMenu(fileName = nameof(GameManagerDataSO), menuName = "SO/GameManager/Data")]
    public sealed class GameManagerDataSO : ScriptableObject
    {
        public MSceneContext[] SceneContexts;
        public AssetReferenceGameObject PlayerRef;
    }
}