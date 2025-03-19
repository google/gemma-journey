// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
                playerManager = FindObjectOfType<PlayerManager>();

                if (playerManager == null)
                {
                    Debug.LogWarning($"SpawnPoint '{spawnPointId}' could not find PlayerManager in the scene.");
                    return;
                }
            }

            // Register this spawn point with the player manager
            playerManager.RegisterSpawnPoint(this);
            Debug.Log($"SpawnPoint '{spawnPointId}' self-registered with PlayerManager.");
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