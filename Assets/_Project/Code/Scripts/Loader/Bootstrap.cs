using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TechnoDemo.Loader
{
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private AssetReference m_sceneRef;

        private void Start()
        {
            Addressables.LoadSceneAsync(m_sceneRef);
        }
    }
}