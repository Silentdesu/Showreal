using TechnoDemo.Core;
using TechnoDemo.Extensions;
using UnityEngine;
using VContainer;

namespace TechnoDemo.Spawn
{
    public interface ISpawnPoint
    {
        MSpawnPoint.ESpawnType Type { get; }
        Transform Transform { get; }
    }
    
    public sealed class MSpawnPoint : MonoBehaviour, ISpawnPoint
    {
        public enum ESpawnType : int
        {
            None = 0,
            Player = 1
        }

        [field: SerializeField] public ESpawnType Type { get; private set; }
        
        public Transform Transform { get; private set; }

        private IGameManager m_gameManager;
        
        [Inject]
        private void Construct(IGameManager gameManager)
        {
            m_gameManager = gameManager;
        }

        private void Start()
        {
            Transform = transform;
            
            if (m_gameManager.GetSpawnedContexts().TryGetObject(out ISpawner spawner))
                spawner.AddSpawnPoint(this);
        }
    }
}