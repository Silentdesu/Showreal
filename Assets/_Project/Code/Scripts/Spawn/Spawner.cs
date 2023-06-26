using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TechnoDemo.Core;
using TechnoDemo.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace TechnoDemo.Spawn
{
    public interface ISpawner
    {
        void AddSpawnPoint(in ISpawnPoint spawnPoint);
        IList<GameObject> GetSpawnedObjects();
        UniTask<GameObject> SpawnGameObjectAsync(AssetReferenceGameObject reference, SpawnPoint.ESpawnType type);
    }

    [DisallowMultipleComponent]
    public sealed class Spawner : SceneContext, ISpawner
    {
        private List<KeyValueX<SpawnPoint.ESpawnType, Transform>> m_spawnPoints = new List<KeyValueX<SpawnPoint.ESpawnType, Transform>>(10);

        private IObjectResolver m_resolver;

        private readonly IList<GameObject> m_spawnedObjects = new List<GameObject>(10);

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            m_resolver = resolver;
            
            this.LogInjectSuccess();
        }

        public void AddSpawnPoint(in ISpawnPoint spawnPoint)
        {
            m_spawnPoints.Add(new KeyValueX<SpawnPoint.ESpawnType, Transform>
            {
                Key = spawnPoint.Type, 
                Value = spawnPoint.Transform
            });
        }

        public IList<GameObject> GetSpawnedObjects() => m_spawnedObjects;

        public async UniTask<GameObject> SpawnGameObjectAsync(
            AssetReferenceGameObject reference,
            SpawnPoint.ESpawnType spawnType)
        {
            await reference.LoadAssetAsync();

            GameObject go = null;
            for (int i = 0, count = m_spawnPoints.Count; i < count; i++)
            {
                if (spawnType != m_spawnPoints[i].Key) continue;

                go = m_resolver.Instantiate(reference.Asset, m_spawnPoints[i].Value.position, Quaternion.identity) as
                    GameObject;
                m_spawnedObjects.Add(go);

                return go;
            }

            return null;
        }
    }
}