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
                playerManager = FindObjectOfType<PlayerManager>();

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

        private void OnGUI()
        {
            if (playerManager == null) return;

            // Display simple instructions in the top-left corner
            GUILayout.BeginArea(new Rect(10, 10, 300, 100));
            GUILayout.Label($"Press {spawnPlayerKey} to spawn player");
            GUILayout.Label($"Press {cycleSpawnPointsKey} to cycle spawn points");

            if (playerManager.SpawnPoints.Count > 0 && currentSpawnPointIndex < playerManager.SpawnPoints.Count)
            {
                GUILayout.Label($"Current spawn point: {playerManager.SpawnPoints[currentSpawnPointIndex].SpawnPointId}");
            }
            else
            {
                GUILayout.Label("No spawn points available");
            }

            GUILayout.EndArea();
        }
    }
}