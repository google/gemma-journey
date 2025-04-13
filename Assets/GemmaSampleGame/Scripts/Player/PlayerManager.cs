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
using Unity.Cinemachine;
using System.Collections.Generic;
using VContainer;
using VContainer.Unity;
using System.Linq;

namespace GoogleDeepMind.GemmaSampleGame
{
    /// <summary>
    /// Manages player-related functionality including spawning and tracking spawn points.
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        // List of all registered spawn points
        private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

        // Reference to the currently active player instance
        private GameObject currentPlayer;
        private Transform playerTransform;

        // Player prefab reference
        [Header("Player Settings")]
        [SerializeField]
        private GameObject playerPrefab;

        // Object resolver for dependency injection
        private IObjectResolver container;

        // Lifetime scope for dependency injection
        [SerializeField]
        private LifetimeScope lifetimeScope;

        // Cinemachine components
        [Header("Cinemachine Components")]
        private CinemachineCamera virtualCamera;
        // Debug HUD settings
        [Header("Debug HUD Settings")]
        [SerializeField]
        private bool showDebugHUD = true;
        [SerializeField]
        private Color debugTextColor = Color.white;
        [SerializeField]
        private Color debugBackgroundColor = new Color(0, 0, 0, 0.7f);
        [SerializeField]
        private int fontSize = 16;
        [SerializeField]
        private Vector2 debugHUDPosition = new Vector2(600, 220); // Position below state machine debug
        [SerializeField]
        private Vector2 debugHUDSize = new Vector2(300, 200);
        [SerializeField]
        private KeyCode toggleDebugHUDKey = KeyCode.F2;

        // Style for the debug text and background
        private GUIStyle debugTextStyle;
        private GUIStyle debugBackgroundStyle;

        /// <summary>
        /// Gets the list of all registered spawn points.
        /// </summary>
        public IReadOnlyList<SpawnPoint> SpawnPoints => spawnPoints.AsReadOnly();

        /// <summary>
        /// Gets the currently active player instance.
        /// </summary>
        public GameObject CurrentPlayer => currentPlayer;

        /// <summary>
        /// Dependency injection method for required game systems.
        /// Receives references to the game state machine and control overlay UI.
        /// </summary>
        /// <param name="gameStateMachine">Reference to the game state machine for state-dependent behavior</param>
        /// <param name="controlOverlayUserInterface">Reference to the control overlay UI for input visualization</param>
        [Inject]
        public void Construct(
            IObjectResolver container
        )
        {
            this.container = container;
        }

        private void Awake()
        {
            // Initialize debug text style
            debugTextStyle = new GUIStyle();
            debugTextStyle.fontSize = fontSize;
            debugTextStyle.normal.textColor = debugTextColor;
            debugTextStyle.wordWrap = true;
            debugTextStyle.padding = new RectOffset(10, 10, 10, 10);

            // Initialize debug background style
            debugBackgroundStyle = new GUIStyle();
            debugBackgroundStyle.normal.background = CreateColorTexture(2, 2, debugBackgroundColor);
        }

        // Helper method to create a colored texture for the background
        private Texture2D CreateColorTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private void Start()
        {
            // Initialize debug text style
            debugTextStyle = new GUIStyle();
            debugTextStyle.fontSize = fontSize;
            debugTextStyle.normal.textColor = debugTextColor;
            debugTextStyle.wordWrap = true;
            debugTextStyle.padding = new RectOffset(10, 10, 10, 10);

            // Initialize debug background style
            debugBackgroundStyle = new GUIStyle();
            debugBackgroundStyle.normal.background = CreateColorTexture(2, 2, debugBackgroundColor);

            // Cache Cinemachine components
            CacheCinemachineComponents();
        }

        /// <summary>
        /// Creates a default spawn point at the origin if no spawn points exist.
        /// </summary>
        private void CreateDefaultSpawnPoint()
        {
            // Create a default spawn point at the origin
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            CreateSpawnPoint(position, rotation, "DefaultSpawnPoint", true, "Automatically created default spawn point");
            Debug.Log("Created default spawn point at origin because no spawn points were found.");
        }

        private void Update()
        {
            // Toggle debug HUD with key press
            if (Input.GetKeyDown(toggleDebugHUDKey))
            {
                showDebugHUD = !showDebugHUD;
            }
        }

        /// <summary>
        /// Registers a spawn point with the player manager.
        /// </summary>
        /// <param name="spawnPoint">The spawn point to register.</param>
        public void RegisterSpawnPoint(SpawnPoint spawnPoint)
        {
            if (!spawnPoints.Contains(spawnPoint))
            {
                spawnPoints.Add(spawnPoint);
                Debug.Log($"Registered spawn point: {spawnPoint.SpawnPointId}");
            }
        }

        /// <summary>
        /// Unregisters a spawn point from the player manager.
        /// </summary>
        /// <param name="spawnPoint">The spawn point to unregister.</param>
        public void UnregisterSpawnPoint(SpawnPoint spawnPoint)
        {
            if (spawnPoints.Contains(spawnPoint))
            {
                spawnPoints.Remove(spawnPoint);
                Debug.Log($"Unregistered spawn point: {spawnPoint.SpawnPointId}");
            }
        }

        /// <summary>
        /// Caches references to all Cinemachine components on the FollowCamera child object.
        /// </summary>
        private void CacheCinemachineComponents()
        {
            Transform followCamera = transform.Find("FollowCamera");
            if (followCamera == null)
            {
                Debug.LogWarning("Cannot cache Cinemachine components: FollowCamera child not found.");
                return;
            }

            virtualCamera = followCamera.GetComponent<CinemachineCamera>();

            // Log any missing components
            if (virtualCamera == null) Debug.LogError("CinemachineCamera not found on FollowCamera");
        }

        /// <summary>
        /// Registers the parent lifetime scope for dependency injection in loaded scenes.
        /// </summary>
        /// <param name="lifetimeScope">The parent lifetime scope to use for dependency injection</param>
        public void RegisterLifetimeScope(LifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }
        /// <summary>
        /// Spawns the player at the default spawn point or at a specific spawn point if provided.
        /// </summary>
        /// <param name="spawnPointId">Optional ID of the spawn point to use. If null, uses the default spawn point.</param>
        /// <returns>The spawned player GameObject, or null if spawning failed.</returns>
        public GameObject SpawnPlayer(string spawnPointId = null)
        {
            // Find the appropriate spawn point
            SpawnPoint spawnPoint = null;

            if (string.IsNullOrEmpty(spawnPointId))
            {
                // Use the default spawn point
                spawnPoint = spawnPoints.FirstOrDefault(sp => sp.IsDefaultSpawnPoint);

                // If no default spawn point is set, use the first available
                if (spawnPoint == null && spawnPoints.Count > 0)
                {
                    spawnPoint = spawnPoints[0];
                }
            }
            else
            {
                // Find the spawn point with the specified ID
                spawnPoint = spawnPoints.FirstOrDefault(sp => sp.SpawnPointId == spawnPointId);
            }

            // Check if a valid spawn point was found
            if (spawnPoint == null)
            {
                Debug.LogWarning("No valid spawn point found for player spawning.");
                return null;
            }

            // Log the spawn action
            Debug.Log($"Player spawned at {spawnPoint.SpawnPointId} ({spawnPoint.Position})");

            // Instantiate the player prefab with automatic dependency injection
            currentPlayer = container.Instantiate(playerPrefab, spawnPoint.Position, spawnPoint.Rotation, transform);
            playerTransform = currentPlayer.transform;

            // Set up camera tracking
            if (virtualCamera != null)
            {
                virtualCamera.Follow = playerTransform;
                // Set this camera as the active camera
                virtualCamera.Priority = -1;
            }

            return currentPlayer;
        }

        public void DespawnPlayer()
        {
            Destroy(currentPlayer);
        }

        /// <summary>
        /// Gets a spawn point by its ID.
        /// </summary>
        /// <param name="spawnPointId">The ID of the spawn point to find.</param>
        /// <returns>The spawn point with the specified ID, or null if not found.</returns>
        public SpawnPoint GetSpawnPoint(string spawnPointId)
        {
            return spawnPoints.FirstOrDefault(sp => sp.SpawnPointId == spawnPointId);
        }

        /// <summary>
        /// Gets the default spawn point.
        /// </summary>
        /// <returns>The default spawn point, or null if none is set.</returns>
        public SpawnPoint GetDefaultSpawnPoint()
        {
            return spawnPoints.FirstOrDefault(sp => sp.IsDefaultSpawnPoint);
        }

        /// <summary>
        /// Creates a new spawn point at the specified position.
        /// </summary>
        /// <param name="position">The position for the spawn point.</param>
        /// <param name="rotation">The rotation for the spawn point.</param>
        /// <param name="spawnPointId">The ID for the spawn point. If null, a unique ID will be generated.</param>
        /// <param name="isDefault">Whether this should be the default spawn point.</param>
        /// <param name="description">Optional description for the spawn point.</param>
        /// <returns>The created spawn point.</returns>
        public SpawnPoint CreateSpawnPoint(Vector3 position, Quaternion rotation, string spawnPointId = null, bool isDefault = false, string description = null)
        {
            // Create a unique ID if none provided
            if (string.IsNullOrEmpty(spawnPointId))
            {
                spawnPointId = $"SpawnPoint_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
            }

            // Create a new GameObject for the spawn point
            GameObject spawnPointObj = new GameObject(spawnPointId);
            spawnPointObj.transform.position = position;
            spawnPointObj.transform.rotation = rotation;

            // Add the SpawnPoint component
            SpawnPoint spawnPoint = spawnPointObj.AddComponent<SpawnPoint>();

            // Set the properties directly
            spawnPoint.spawnPointId = spawnPointId;
            spawnPoint.isDefaultSpawnPoint = isDefault;
            if (!string.IsNullOrEmpty(description))
            {
                spawnPoint.description = description;
            }

            // The SpawnPoint will register itself with this PlayerManager

            Debug.Log($"Created new spawn point: {spawnPointId} at {position}");
            return spawnPoint;
        }

        private void OnGUI()
        {
            if (!showDebugHUD) return;

            // Draw background
            GUI.Box(new Rect(debugHUDPosition.x, debugHUDPosition.y, debugHUDSize.x, debugHUDSize.y), "", debugBackgroundStyle);

            // Build debug information string
            string debugInfo = "<b>PLAYER MANAGER DEBUG</b>\n\n";

            // Show spawn points information
            debugInfo += $"<b>Spawn Points:</b> {spawnPoints.Count}\n\n";

            if (spawnPoints.Count > 0)
            {
                foreach (var sp in spawnPoints)
                {
                    string defaultTag = sp.IsDefaultSpawnPoint ? " (DEFAULT)" : "";
                    debugInfo += $"- {sp.SpawnPointId}{defaultTag}: {sp.Position}\n";
                    if (!string.IsNullOrEmpty(sp.Description))
                    {
                        debugInfo += $"  <i>{sp.Description}</i>\n";
                    }
                }
            }
            else
            {
                debugInfo += "No spawn points registered.\n";
            }

            // Show current player information
            debugInfo += $"\n<b>Current Player:</b> {(currentPlayer != null ? "Active" : "None")}\n";

            // Add toggle hint
            debugInfo += $"\n<i>Press {toggleDebugHUDKey} to toggle this display</i>";

            // Display the debug information
            debugTextStyle.richText = true;
            GUI.Label(new Rect(debugHUDPosition.x, debugHUDPosition.y, debugHUDSize.x, debugHUDSize.y), debugInfo, debugTextStyle);
        }
    }
}
