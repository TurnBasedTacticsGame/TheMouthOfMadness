using UnityEngine;

namespace Source.GameEvents.Core
{
    public class GameContext : MonoBehaviour
    {
        public VirtualAudioSource audioSourcePrefab;

        public bool IsPaused { get; set; }
        public SpawnPoint ActiveSpawnPoint { get; set; }
    }
}
