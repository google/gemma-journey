/**
 * AnimalCrossingCameraExtensions.cs
 * 
 * This class provides additional camera behaviors specific to Animal Crossing style cameras.
 * It works alongside the AnimalCrossingCameraSystem to add features like:
 * - Fixed camera rotation with slight adjustments
 * - Smooth camera movement during player rotation
 * - Special camera behaviors for specific areas
 */
using UnityEngine;
using Unity.Cinemachine;
using System.Reflection;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class AnimalCrossingCameraExtensions : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AnimalCrossingCameraSystem mainCameraSystem;
        [SerializeField] private Transform playerTransform;

        [Header("Camera Rotation Settings")]
        [Tooltip("Whether to allow slight camera rotation adjustments")]
        [SerializeField] private bool allowRotationAdjustment = true;
        [Tooltip("Maximum angle the camera can rotate from its default position")]
        [SerializeField] private float maxRotationAdjustment = 15f;
        [Tooltip("Speed of camera rotation adjustment")]
        [SerializeField] private float rotationAdjustmentSpeed = 2f;

        [Header("Special Area Settings")]
        [Tooltip("Whether to use special camera settings for marked areas")]
        [SerializeField] private bool useSpecialAreas = true;
        [Tooltip("Layer mask for special camera area triggers")]
        [SerializeField] private LayerMask specialAreaLayer;

        // Internal variables
        private float currentRotationOffset = 0f;
        private float targetRotationOffset = 0f;
        private Vector3 lastPlayerPosition;
        private Quaternion lastPlayerRotation;
        private bool inSpecialArea = false;
        private SpecialCameraArea currentSpecialArea = null;

        private void Start()
        {
            // Find references if not assigned
            if (mainCameraSystem == null)
            {
                mainCameraSystem = FindObjectOfType<AnimalCrossingCameraSystem>();
                if (mainCameraSystem == null)
                {
                    Debug.LogError("AnimalCrossingCameraExtensions: No AnimalCrossingCameraSystem found in the scene!");
                }
            }

            if (playerTransform == null && mainCameraSystem != null)
            {
                // Try to get the player transform from the camera system's target
                var cameraSystemField = mainCameraSystem.GetType().GetField("target", BindingFlags.NonPublic | BindingFlags.Instance);
                if (cameraSystemField != null)
                {
                    playerTransform = cameraSystemField.GetValue(mainCameraSystem) as Transform;
                }

                if (playerTransform == null)
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        playerTransform = player.transform;
                    }
                    else
                    {
                        Debug.LogError("AnimalCrossingCameraExtensions: Player not found!");
                    }
                }
            }

            if (playerTransform != null)
            {
                lastPlayerPosition = playerTransform.position;
                lastPlayerRotation = playerTransform.rotation;
            }
        }

        private void Update()
        {
            if (playerTransform == null || mainCameraSystem == null) return;

            // Handle rotation adjustments
            if (allowRotationAdjustment)
            {
                HandleRotationAdjustment();
            }

            // Update last known player position and rotation
            lastPlayerPosition = playerTransform.position;
            lastPlayerRotation = playerTransform.rotation;
        }

        /// <summary>
        /// Handles slight camera rotation adjustments based on player movement
        /// </summary>
        private void HandleRotationAdjustment()
        {
            // Calculate player movement direction
            Vector3 movementDirection = playerTransform.position - lastPlayerPosition;

            if (movementDirection.magnitude > 0.01f)
            {
                // Project movement onto horizontal plane
                movementDirection.y = 0;
                movementDirection.Normalize();

                // Calculate angle between forward and movement direction
                Vector3 playerForward = playerTransform.forward;
                playerForward.y = 0;
                playerForward.Normalize();

                // Calculate the signed angle for rotation
                float angle = Vector3.SignedAngle(playerForward, movementDirection, Vector3.up);

                // Limit the angle to the maximum adjustment
                targetRotationOffset = Mathf.Clamp(angle * 0.5f, -maxRotationAdjustment, maxRotationAdjustment);
            }
            else
            {
                // Gradually return to center when not moving
                targetRotationOffset = Mathf.Lerp(targetRotationOffset, 0f, Time.deltaTime * 2f);
            }

            // Smoothly adjust the current rotation
            currentRotationOffset = Mathf.Lerp(currentRotationOffset, targetRotationOffset,
                Time.deltaTime * rotationAdjustmentSpeed);

            // Apply the rotation to the camera
            ApplyCameraRotation();
        }

        /// <summary>
        /// Applies the calculated rotation to the camera system
        /// </summary>
        private void ApplyCameraRotation()
        {
            // Access the outdoor and indoor cameras from the main system
            var outdoorCameraField = mainCameraSystem.GetType().GetField("outdoorCamera",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var indoorCameraField = mainCameraSystem.GetType().GetField("indoorCamera",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (outdoorCameraField != null && indoorCameraField != null)
            {
                CinemachineCamera outdoorCamera = outdoorCameraField.GetValue(mainCameraSystem) as CinemachineCamera;
                CinemachineCamera indoorCamera = indoorCameraField.GetValue(mainCameraSystem) as CinemachineCamera;

                if (outdoorCamera != null && indoorCamera != null)
                {
                    // Get the base rotation values
                    var outdoorPitchField = mainCameraSystem.GetType().GetField("outdoorPitch",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    var indoorPitchField = mainCameraSystem.GetType().GetField("indoorPitch",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    if (outdoorPitchField != null && indoorPitchField != null)
                    {
                        float outdoorPitch = (float)outdoorPitchField.GetValue(mainCameraSystem);
                        float indoorPitch = (float)indoorPitchField.GetValue(mainCameraSystem);

                        // Apply the rotation offset to both cameras
                        outdoorCamera.transform.eulerAngles = new Vector3(
                            outdoorPitch,
                            currentRotationOffset,
                            0f
                        );

                        indoorCamera.transform.eulerAngles = new Vector3(
                            indoorPitch,
                            currentRotationOffset,
                            0f
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Called when the player enters a trigger collider
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!useSpecialAreas) return;

            // Check if this is a special camera area
            if (((1 << other.gameObject.layer) & specialAreaLayer) != 0)
            {
                SpecialCameraArea specialArea = other.GetComponent<SpecialCameraArea>();
                if (specialArea != null)
                {
                    inSpecialArea = true;
                    currentSpecialArea = specialArea;
                    ApplySpecialAreaSettings(specialArea);
                }
            }
        }

        /// <summary>
        /// Called when the player exits a trigger collider
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (!useSpecialAreas || !inSpecialArea) return;

            // Check if this is the current special camera area
            if (((1 << other.gameObject.layer) & specialAreaLayer) != 0)
            {
                SpecialCameraArea specialArea = other.GetComponent<SpecialCameraArea>();
                if (specialArea != null && specialArea == currentSpecialArea)
                {
                    inSpecialArea = false;
                    currentSpecialArea = null;
                    ResetToDefaultSettings();
                }
            }
        }

        /// <summary>
        /// Applies camera settings for a special area
        /// </summary>
        private void ApplySpecialAreaSettings(SpecialCameraArea specialArea)
        {
            // Apply special area settings to the camera system
            if (specialArea.overrideIndoorOutdoor)
            {
                mainCameraSystem.SetEnvironment(specialArea.treatAsIndoor);
            }

            // Additional special area settings could be applied here
        }

        /// <summary>
        /// Resets camera settings to default after leaving a special area
        /// </summary>
        private void ResetToDefaultSettings()
        {
            // Reset any special settings that were applied
            // The main camera system will handle indoor/outdoor detection automatically
        }
    }

    /// <summary>
    /// Component to define a special camera area with custom settings
    /// </summary>
    public class SpecialCameraArea : MonoBehaviour
    {
        [Tooltip("Name of this special camera area")]
        public string areaName = "Special Area";

        [Tooltip("Whether to override the indoor/outdoor detection")]
        public bool overrideIndoorOutdoor = false;

        [Tooltip("Treat this area as indoor even if no roof is detected")]
        public bool treatAsIndoor = false;

        [Tooltip("Custom camera angle for this area (if used)")]
        public float customCameraPitch = 45f;

        [Tooltip("Whether to use a custom camera angle")]
        public bool useCustomCameraPitch = false;
    }
}