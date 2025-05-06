using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GoogleDeepMind.GemmaSampleGame
{
    /// <summary>
    /// Represents a spawn point in the game world where the player can be spawned.
    /// Automatically registers itself with the PlayerManager.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        public string spawnPointId = "Default";

        [SerializeField]
        public bool isDefaultSpawnPoint = false;

        [SerializeField]
        [Tooltip("Optional description of this spawn point's location or purpose")]
        public string description;

        /// <summary>
        /// Gets the unique identifier for this spawn point.
        /// </summary>
        public string SpawnPointId => spawnPointId;

        /// <summary>
        /// Gets whether this is the default spawn point.
        /// </summary>
        public bool IsDefaultSpawnPoint => isDefaultSpawnPoint;

        /// <summary>
        /// Gets the description of this spawn point.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Gets the position of this spawn point in world space.
        /// </summary>
        public Vector3 Position => transform.position;

        /// <summary>
        /// Gets the rotation of this spawn point.
        /// </summary>
        public Quaternion Rotation => transform.rotation;

        private PlayerManager playerManager;

        private void Start()
        {
            // Find the PlayerManager in the scene
            FindAndRegisterWithPlayerManager();
        }

        private void FindAndRegisterWithPlayerManager()
        {
            // Try to find the PlayerManager in the scene
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>();

                if (playerManager == null)
                {
                    return;
                }
            }

            // Register this spawn point with the player manager
            playerManager.RegisterSpawnPoint(this);
        }

        private void OnDestroy()
        {
            // Unregister this spawn point when destroyed
            if (playerManager != null)
            {
                playerManager.UnregisterSpawnPoint(this);
            }
        }

        private void OnDrawGizmos()
        {
            // Draw a visual indicator for the spawn point in the editor
            Gizmos.color = isDefaultSpawnPoint ? Color.green : Color.blue;
            Gizmos.DrawSphere(transform.position, 0.5f);
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }

        // This method is still here for VContainer support if available,
        // but the class no longer depends on it
        [Inject]
        public void Construct(PlayerManager playerManager)
        {
            this.playerManager = playerManager;

            // Register this spawn point with the player manager
            playerManager.RegisterSpawnPoint(this);
        }
    }
}