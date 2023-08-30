using UnityEngine;

namespace _Project.Code.Scripts.Utils
{
    [DisallowMultipleComponent]
    public sealed class NameGiverBehaviour : MonoBehaviour
    {
        private void OnValidate()
        {
            transform.name = GetComponent<MeshRenderer>().sharedMaterial.name.Replace("(Instance)", "");
        }
    }
}