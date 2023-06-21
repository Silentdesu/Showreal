using UnityEngine;

namespace TechnoDemo.Spawn
{
    public class MSpawnPoint : MonoBehaviour
    {
        public enum ESpawnType : int
        {
            None = 0,
            Player = 1
        }

        [SerializeField] private ESpawnType m_type = ESpawnType.None;

        public ESpawnType Type => m_type;
    }
}