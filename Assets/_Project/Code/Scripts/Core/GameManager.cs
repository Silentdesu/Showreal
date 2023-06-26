using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TechnoDemo.Extensions;
using TechnoDemo.Player;
using TechnoDemo.Scopes;
using TechnoDemo.Spawn;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TechnoDemo.Core
{
    public interface IGameManager
    {
        SceneScopeContainerSO ContainerSO { get; }
        IList<GameObject> GetSpawnedContexts();
    }

    public sealed class GameManager : IGameManager, IInitializable, IStartable, IDisposable
    {
        private readonly IObjectResolver m_resolver;
        private readonly SceneScopeContainerSO m_container;

        private readonly IList<GameObject> m_spawnedContexts = new List<GameObject>(5);
        private readonly IDisposable m_disposable;

        public SceneScopeContainerSO ContainerSO => m_container;
        
        [Inject]
        public GameManager(IObjectResolver resolver, SceneScopeContainerSO container)
        {
            m_resolver = resolver;
            m_container = container;
        }

        public void Initialize()
        {
            var sceneContext = new GameObject("Scene Contexts");

            Span<SceneContext> contexts = m_container.GameManagerSo.SceneContexts;

            for (int i = 0, count = contexts.Length; i < count; i++)
            {
                m_spawnedContexts.Add(m_resolver.Instantiate(m_container.GameManagerSo.SceneContexts[i].gameObject, sceneContext.transform));
            }
        }
        
        public void Start()
        {
            SpawnPlayer().Forget();
        }

        private async UniTaskVoid SpawnPlayer()
        {
            if (m_spawnedContexts.TryGetObject(out ISpawner spawner))
            {
                var go = await spawner.SpawnGameObjectAsync(m_container.GameManagerSo.PlayerRef, SpawnPoint.ESpawnType.Player);

                if (go.TryGetComponent(out IPlayer player)) player.Inject(this);
            }
        }

        public IList<GameObject> GetSpawnedContexts() => m_spawnedContexts;

        public void Dispose()
        {
            m_resolver?.Dispose();
            m_disposable?.Dispose();
        }
    }
}