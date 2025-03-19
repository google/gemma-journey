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

/**
 * ClueCameraController.cs
 * 
 * This class manages the third-person camera behavior in the game.
 * It follows a target (typically the player character) with configurable offset,
 * supports smooth zooming in/out with mouse scroll wheel, and allows camera rotation
 * when holding the right mouse button.
 * 
 * The camera maintains a consistent view of the target while providing smooth transitions
 * between different positions and zoom levels.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class ClueCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -8f);

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoomDistance = 5f;
    [SerializeField] private float maxZoomDistance = 12f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomSmoothness = 5f;

    private float currentZoomDistance;
    private float targetZoomDistance;
    private float currentRotationAngle;
    private Vector3 smoothVelocity;
    private float smoothZoomVelocity;

    /// <summary>
    /// Initializes the camera controller, finds the target if not assigned,
    /// and sets up initial zoom and rotation values.
    /// </summary>
    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to camera controller! Attempting to find player...");
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        // Initialize zoom
        currentZoomDistance = offset.magnitude;
        targetZoomDistance = currentZoomDistance;

        // Initialize rotation
        currentRotationAngle = transform.eulerAngles.y;

        // Ensure initial position
        UpdateCameraPosition(true);
    }

    /// <summary>
    /// Updates the camera position and rotation after all other updates.
    /// This ensures the camera follows the target smoothly and consistently.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        HandleZoom();
        HandleRotation();
        UpdateCameraPosition();
    }

    /// <summary>
    /// Processes mouse scroll wheel input to adjust camera zoom level.
    /// Applies smoothing to zoom transitions for a more polished feel.
    /// </summary>
    private void HandleZoom()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            targetZoomDistance = Mathf.Clamp(
                targetZoomDistance - scrollDelta * zoomSpeed,
                minZoomDistance,
                maxZoomDistance
            );
        }

        currentZoomDistance = Mathf.SmoothDamp(
            currentZoomDistance,
            targetZoomDistance,
            ref smoothZoomVelocity,
            1f / zoomSmoothness
        );
    }

    /// <summary>
    /// Handles camera rotation around the target when the right mouse button is held.
    /// Rotation occurs on the horizontal plane only.
    /// </summary>
    private void HandleRotation()
    {
        // Optional: Hold right mouse button to rotate
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            currentRotationAngle += mouseX * rotationSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Updates the camera's position based on target position, current zoom level, and rotation.
    /// Maintains a smooth follow effect unless instant positioning is requested.
    /// </summary>
    /// <param name="instant">If true, camera immediately jumps to the target position without smoothing</param>
    private void UpdateCameraPosition(bool instant = false)
    {
        // Calculate desired position
        Vector3 normalizedOffset = offset.normalized;
        Vector3 scaledOffset = normalizedOffset * currentZoomDistance;

        // Rotate offset based on current rotation
        Vector3 rotatedOffset = Quaternion.Euler(0f, currentRotationAngle, 0f) * scaledOffset;
        Vector3 targetPosition = target.position + rotatedOffset;

        if (instant)
        {
            transform.position = targetPosition;
        }
        else
        {
            // Smoothly move camera
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref smoothVelocity,
                1f / followSpeed
            );
        }

        // Always look at target
        transform.LookAt(target.position + Vector3.up * 1f); // Offset slightly up from feet
    }

    /// <summary>
    /// Forces the camera to immediately update its position to follow the target.
    /// Useful for teleportation, scene transitions, or initial positioning.
    /// </summary>
    public void SnapToTarget()
    {
        UpdateCameraPosition(true);
    }
}
}