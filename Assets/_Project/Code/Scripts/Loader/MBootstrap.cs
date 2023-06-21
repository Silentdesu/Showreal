using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TechnoDemo.Loader
{
    public sealed class MBootstrap : MonoBehaviour
    {
        [SerializeField] private string m_sceneName = "Sandbox";
    
        private void Start()
        {
            Addressables.LoadSceneAsync(m_sceneName);
        }
    }
}
