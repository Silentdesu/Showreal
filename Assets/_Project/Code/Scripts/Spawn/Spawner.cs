using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TechnoDemo.Player;
using TechnoDemo.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace TechnoDemo.Spawn
{
    public interface ISpawner
    {
        T GetObject<T>() where T : class;
        bool TryGetObject<T>(out T obj) where T : class;
        UniTask<T> GetObjectAsync<T>() where T : class;
        UniTask<Tuple<bool, T>> TryGetObjectAsync<T>() where T : class;
    }

    [DisallowMultipleComponent]
    public sealed class Spawner : MonoBehaviour, ISpawner
    {
        [SerializeField] private KeyValueX<SpawnPoint.ESpawnType, Transform>[] m_spawnPoints;

        private IObjectResolver m_resolver;
        private SpawnerDataSO m_settings;

        private readonly IList<GameObject> m_spawnedObjects = new List<GameObject>(10);
        
        [Inject]
        private void Construct(IObjectResolver resolver, SpawnerDataSO settings)
        {
            m_resolver = resolver;
            m_settings = settings;
            this.LogInjectSuccess();
        }

        private void Start()
        {
            SpawnGameObjectAsync(m_settings.PlayerRef, SpawnPoint.ESpawnType.Player).Forget();
        }

        private async UniTask<GameObject> SpawnGameObjectAsync(
            AssetReferenceGameObject @ref,
            SpawnPoint.ESpawnType spawnType)
        {
            await @ref.LoadAssetAsync();

            GameObject go = null;
            for (int i = 0, count = m_spawnPoints.Length; i < count; i++)
            {
                if (spawnType != m_spawnPoints[i].Key) continue;

                go = m_resolver.Instantiate(@ref.Asset, m_spawnPoints[i].Value.position, Quaternion.identity) as
                    GameObject;
                m_spawnedObjects.Add(go);
                
                return go;
            }

            return null;
        }

        public T GetObject<T>() where T : class
        {
            Span<GameObject> objs = m_spawnedObjects.ToArray();
            
            for (int i = 0, count = objs.Length; i < count; i++)
                if (objs[i].TryGetComponent(out T comp))
                    return comp;

            return null;
        }

        public bool TryGetObject<T>(out T obj) where T : class
        {
            obj = GetObject<T>();

            return obj != null;
        }

        public async UniTask<T> GetObjectAsync<T>() where T : class
        {
            T obj = null;
            await UniTask.WaitUntil(() => (obj = GetObject<T>()) != null);

            return obj;
        }

        public async UniTask<Tuple<bool, T>> TryGetObjectAsync<T>() where T : class
        {
            T obj = null;
            await UniTask.WaitUntil(() => (obj = GetObject<T>()) != null);

            return new Tuple<bool, T>(obj != null, obj);
        }
    }
}