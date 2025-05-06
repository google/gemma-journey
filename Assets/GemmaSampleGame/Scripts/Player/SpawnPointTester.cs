using UnityEngine;
using VContainer;
using System.Collections.Generic;

namespace GoogleDeepMind.GemmaSampleGame
{
    /// <summary>
    /// A simple test script to demonstrate the spawn point and player manager functionality.
    /// </summary>
    public class SpawnPointTester : MonoBehaviour
    {
        [Inject]
        private PlayerManager playerManager;

        [SerializeField]
        private KeyCode spawnPlayerKey = KeyCode.P;

        [SerializeField]
        private KeyCode cycleSpawnPointsKey = KeyCode.N;

        private int currentSpawnPointIndex = 0;

        private void Start()
        {
            // Find the PlayerManager if not injected
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>();

                if (playerManager == null)
                {
                    Debug.LogError("SpawnPointTester could not find PlayerManager in the scene.");
                    enabled = false; // Disable this component
                    return;
                }
            }
        }

        private void Update()
        {
            // Spawn player at the current/default spawn point
            if (Input.GetKeyDown(spawnPlayerKey))
            {
                if (playerManager.SpawnPoints.Count > 0)
                {
                    string spawnPointId = null;

                    if (currentSpawnPointIndex < playerManager.SpawnPoints.Count)
                    {
                        spawnPointId = playerManager.SpawnPoints[currentSpawnPointIndex].SpawnPointId;
                    }

                    playerManager.SpawnPlayer(spawnPointId);
                }
                else
                {
                    Debug.LogWarning("No spawn points available.");
                }
            }

            // Cycle through available spawn points
            if (Input.GetKeyDown(cycleSpawnPointsKey) && playerManager.SpawnPoints.Count > 0)
            {
                currentSpawnPointIndex = (currentSpawnPointIndex + 1) % playerManager.SpawnPoints.Count;
                Debug.Log($"Selected spawn point: {playerManager.SpawnPoints[currentSpawnPointIndex].SpawnPointId}");
            }
        }

    }
}