using UnityEngine;

namespace TechnoDemo.Player
{
    public interface IPlayer
    {
        Transform Transform { get; }
    }
    
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour, IPlayer
    {
        public Transform Transform { get; private set; }

        private void Awake()
        {
            Transform = transform;
        }

        private void OnDestroy()
        {
            Transform = null;
        }
    }
}