using System.Collections.Generic;
using TechnoDemo.Extensions;
using TechnoDemo.Spawn;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TechnoDemo.Core
{
    public interface IGameManager
    {
        GameManagerDataSO Settings { get; }
        IList<GameObject> GetSpawnedContexts();
    }

    public sealed class CGameManager : IGameManager, IInitializable, IStartable
    {
        private IObjectResolver m_resolver;
        private GameManagerDataSO m_settings;

        private IList<GameObject> m_spawnedContexts = new List<GameObject>(5);

        public GameManagerDataSO Settings => m_settings;
        
        [Inject]
        public CGameManager(IObjectResolver resolver, GameManagerDataSO settings)
        {
            m_resolver = resolver;
            m_settings = settings;
        }

        public void Initialize()
        {
            var sceneContext = new GameObject("Scene Contexts");

            System.Span<MSceneContext> contexts = m_settings.SceneContexts;

            for (int i = 0, count = contexts.Length; i < count; i++)
            {
                m_spawnedContexts.Add(m_resolver.Instantiate(m_settings.SceneContexts[i].gameObject, sceneContext.transform));
            }
        }
        
        public void Start()
        {
            if (m_spawnedContexts.TryGetObject(out ISpawner spawner))
            {
                spawner.SpawnGameObjectAsync(m_settings.PlayerRef, MSpawnPoint.ESpawnType.Player);
            }
        }

        public IList<GameObject> GetSpawnedContexts() => m_spawnedContexts;
    }
}