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

namespace GoogleDeepMind.GemmaSampleGame
{
    public class PortalDoorManager : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera portalCamera;

        [Header("Door References")]
        [SerializeField] private DoorManager doorManager;

        [Inject]
        private LevelManager levelManager;

        private int currentRoomIndex = 0;
        private int nextRoomIndex = 1;

        // Layer masks for different room layers
        private readonly int defaultLayer = LayerMask.NameToLayer("Default");
        private readonly int uiLayer = LayerMask.NameToLayer("UI");
        private readonly int playerLayer = LayerMask.NameToLayer("Player");
        private readonly int activeDoorLayer = LayerMask.NameToLayer("ActiveDoor");

        private void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }

            if (portalCamera == null || doorManager == null)
            {
                Debug.LogError("PortalDoorManager: Portal camera or door manager not assigned!");
                enabled = false;
                return;
            }

            // Set initial camera masks
            UpdateCameraMasks();
        }

        private void OnEnable()
        {
            if (doorManager != null)
            {
                // Subscribe to door entry event
                doorManager.enabled = true;
            }
        }

        private void OnDisable()
        {
            if (doorManager != null)
            {
                doorManager.enabled = false;
            }
        }

        private void Update()
        {
            // Check for door entry input
            if (levelManager.CurrentRoomManager != null)
            {
                HandleDoorEntry();
            }
        }

        private void HandleDoorEntry()
        {
            if (doorManager.IsOpen && Input.GetKeyDown(KeyCode.E))
            {
                // Change room and update camera masks
                ChangeRoom();
            }
        }

        private void ChangeRoom()
        {
            // Update room indices
            currentRoomIndex = nextRoomIndex;
            nextRoomIndex = (nextRoomIndex + 1) % 6; // Assuming 6 rooms (Room0 to Room5)

            // Update level manager
            levelManager.ChangeLevel();

            // Update camera masks
            UpdateCameraMasks();

            // Hide the door
            doorManager.Close();
            doorManager.enabled = false;
        }

        private void UpdateCameraMasks()
        {
            // Set up player camera mask (default layers + current room)
            int playerCameraMask = (1 << defaultLayer) | (1 << uiLayer) |
                                 (1 << playerLayer) | (1 << activeDoorLayer) |
                                 (1 << LayerMask.NameToLayer($"Room{currentRoomIndex}"));

            // Set up portal camera mask (only next room)
            int portalCameraMask = 1 << LayerMask.NameToLayer($"Room{nextRoomIndex}");

            // Apply masks
            playerCamera.cullingMask = playerCameraMask;
            portalCamera.cullingMask = portalCameraMask;
        }
    }
}