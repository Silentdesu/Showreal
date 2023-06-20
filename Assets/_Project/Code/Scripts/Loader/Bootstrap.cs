using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TechnoDemo.Loader
{
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private string m_sceneName = "Sandbox";
    
        private void Start()
        {
            Addressables.LoadSceneAsync(m_sceneName);
        }
    }
}
