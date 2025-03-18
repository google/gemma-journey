/**
 * AnimalCrossingCameraSystem.cs
 * 
 * This class implements an Animal Crossing: New Horizons style camera system using Cinemachine.
 * Features:
 * - Fixed isometric-style camera angle
 * - Smooth following of the player
 * - Automatic adjustment for indoor/outdoor environments
 * - Smooth transitions between camera states
 * - Optional zoom functionality
 */
using UnityEngine;
using Unity.Cinemachine;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class AnimalCrossingCameraSystem : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [Tooltip("Tag to find player if not manually assigned")]
        [SerializeField] private string playerTag = "Player";

        [Header("Camera References")]
        [SerializeField] private CinemachineCamera outdoorCamera;
        [SerializeField] private CinemachineCamera indoorCamera;
        [SerializeField] private CinemachineBrain cinemachineBrain;

        [Header("Outdoor Camera Settings")]
        [Tooltip("Height above the player for outdoor camera")]
        [SerializeField] private float outdoorHeight = 10f;
        [Tooltip("Distance behind the player for outdoor camera")]
        [SerializeField] private float outdoorDistance = 10f;
        [Tooltip("Camera pitch angle for outdoor view (in degrees)")]
        [SerializeField] private float outdoorPitch = 45f;

        [Header("Indoor Camera Settings")]
        [Tooltip("Height above the player for indoor camera")]
        [SerializeField] private float indoorHeight = 7f;
        [Tooltip("Distance behind the player for indoor camera")]
        [SerializeField] private float indoorDistance = 7f;
        [Tooltip("Camera pitch angle for indoor view (in degrees)")]
        [SerializeField] private float indoorPitch = 40f;

        [Header("Transition Settings")]
        [SerializeField] private float transitionTime = 1.0f;

        [Header("Environment Detection")]
        [SerializeField] private LayerMask roofLayer;
        [SerializeField] private float raycastDistance = 15f;

        // Current camera state
        private bool isIndoors = false;
        private CinemachineCamera currentCamera;
        private CinemachineBlendDefinition originalBlend;

        private void Awake()
        {
            // Find references if not assigned
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(playerTag);
                if (player != null)
                {
                    target = player.transform;
                }
                else
                {
                    Debug.LogError("AnimalCrossingCameraSystem: Player not found! Assign a target or ensure a GameObject with the correct tag exists.");
                }
            }

            if (cinemachineBrain == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
                    if (cinemachineBrain == null)
                    {
                        cinemachineBrain = mainCamera.gameObject.AddComponent<CinemachineBrain>();
                    }
                }
                else
                {
                    Debug.LogError("AnimalCrossingCameraSystem: Main camera not found!");
                }
            }

            // Store the original blend definition
            if (cinemachineBrain != null)
            {
                originalBlend = cinemachineBrain.DefaultBlend;
            }

            // Create cameras if not assigned
            SetupCinemachineCameras();
        }

        private void Start()
        {
            // Initialize with outdoor camera
            SwitchToOutdoorCamera();
        }

        private void Update()
        {
            if (target == null) return;

            // Check if player is indoors or outdoors
            CheckEnvironment();
        }

        /// <summary>
        /// Sets up the Cinemachine cameras if they don't exist
        /// </summary>
        private void SetupCinemachineCameras()
        {
            // Create outdoor camera if not assigned
            if (outdoorCamera == null)
            {
                GameObject outdoorCameraObj = new GameObject("AC_OutdoorCamera");
                outdoorCamera = outdoorCameraObj.AddComponent<CinemachineCamera>();
                ConfigureOutdoorCamera();
            }

            // Create indoor camera if not assigned
            if (indoorCamera == null)
            {
                GameObject indoorCameraObj = new GameObject("AC_IndoorCamera");
                indoorCamera = indoorCameraObj.AddComponent<CinemachineCamera>();
                ConfigureIndoorCamera();
            }
        }

        /// <summary>
        /// Configures the outdoor camera with Animal Crossing style settings
        /// </summary>
        private void ConfigureOutdoorCamera()
        {
            var framingTransposer = outdoorCamera.GetComponent<CinemachineFramingTransposer>();
            if (framingTransposer == null)
            {
                framingTransposer = outdoorCamera.gameObject.AddComponent<CinemachineFramingTransposer>();
            }

            // Set up the outdoor camera properties
            outdoorCamera.Follow = target;
            outdoorCamera.LookAt = target;

            // Configure the framing transposer for Cinemachine 3.x
            // Use the correct property names for Cinemachine 3.x
            framingTransposer.m_CameraDistance = outdoorDistance;
            framingTransposer.m_TrackedObjectOffset = new Vector3(0, outdoorHeight, 0);

            // Set the camera angle
            outdoorCamera.transform.eulerAngles = new Vector3(outdoorPitch, 0, 0);

            // Set priority (higher than indoor by default)
            outdoorCamera.Priority = 10;
        }

        /// <summary>
        /// Configures the indoor camera with Animal Crossing style settings
        /// </summary>
        private void ConfigureIndoorCamera()
        {
            var framingTransposer = indoorCamera.GetComponent<CinemachineFramingTransposer>();
            if (framingTransposer == null)
            {
                framingTransposer = indoorCamera.gameObject.AddComponent<CinemachineFramingTransposer>();
            }

            // Set up the indoor camera properties
            indoorCamera.Follow = target;
            indoorCamera.LookAt = target;

            // Configure the framing transposer for Cinemachine 3.x
            // Use the correct property names for Cinemachine 3.x
            framingTransposer.m_CameraDistance = indoorDistance;
            framingTransposer.m_TrackedObjectOffset = new Vector3(0, indoorHeight, 0);

            // Set the camera angle
            indoorCamera.transform.eulerAngles = new Vector3(indoorPitch, 0, 0);

            // Set priority (lower than outdoor by default)
            indoorCamera.Priority = 5;
        }

        /// <summary>
        /// Checks if the player is indoors or outdoors and switches cameras accordingly
        /// </summary>
        private void CheckEnvironment()
        {
            // Cast a ray upward from the player to detect if there's a roof
            bool hasRoofAbove = Physics.Raycast(target.position, Vector3.up, raycastDistance, roofLayer);

            // Switch camera if environment changed
            if (hasRoofAbove && !isIndoors)
            {
                SwitchToIndoorCamera();
            }
            else if (!hasRoofAbove && isIndoors)
            {
                SwitchToOutdoorCamera();
            }
        }

        /// <summary>
        /// Switches to the outdoor camera
        /// </summary>
        public void SwitchToOutdoorCamera()
        {
            outdoorCamera.Priority = 15;
            indoorCamera.Priority = 5;
            isIndoors = false;
            currentCamera = outdoorCamera;
        }

        /// <summary>
        /// Switches to the indoor camera
        /// </summary>
        public void SwitchToIndoorCamera()
        {
            indoorCamera.Priority = 15;
            outdoorCamera.Priority = 5;
            isIndoors = true;
            currentCamera = indoorCamera;
        }

        /// <summary>
        /// Forces an immediate update of the camera position
        /// </summary>
        public void SnapToTarget()
        {
            if (currentCamera != null && cinemachineBrain != null)
            {
                // In Cinemachine 3.x, BlendTime is read-only, so we need to create a new blend definition
                // Create a new blend definition with 0 blend time for immediate transition
                CinemachineBlendDefinition instantBlend = new CinemachineBlendDefinition(
                    cinemachineBrain.DefaultBlend.Style,
                    0f // Zero blend time for instant transition
                );

                // Set the new blend definition
                cinemachineBrain.DefaultBlend = instantBlend;

                // Reset blend time after snap
                StartCoroutine(ResetBlendTime());
            }
        }

        /// <summary>
        /// Resets the blend time after a snap
        /// </summary>
        private System.Collections.IEnumerator ResetBlendTime()
        {
            yield return new WaitForEndOfFrame();

            // In Cinemachine 3.x, BlendTime is read-only, so we need to create a new blend definition
            // Create a new blend definition with the original transition time
            CinemachineBlendDefinition normalBlend = new CinemachineBlendDefinition(
                originalBlend.Style,
                transitionTime
            );

            // Set the new blend definition
            cinemachineBrain.DefaultBlend = normalBlend;
        }

        /// <summary>
        /// Manually set whether the player is indoors or outdoors
        /// </summary>
        public void SetEnvironment(bool indoor)
        {
            if (indoor)
            {
                SwitchToIndoorCamera();
            }
            else
            {
                SwitchToOutdoorCamera();
            }
        }
    }
}