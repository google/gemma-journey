using UnityEngine;
using System.Collections.Generic;
using VContainer;
using GoogleDeepMind.GemmaSampleGame.StateManagement;
using Cysharp.Threading.Tasks;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class PortalDoor : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem portalEffect;
        [SerializeField] public bool isActive = false;
        [SerializeField] public LayerMask playerLayer;
        [SerializeField] private BoxCollider triggerVolume;
        [SerializeField] private Collider doorCollider;

        [Header("Portal View")]
        [SerializeField] private Camera portalCamera;
        [SerializeField] private RenderTexture portalRenderTexture;
        [SerializeField] private MeshRenderer portalQuad;
        [SerializeField] private LayerMask portalViewMask; // What layers to render in portal view
        [SerializeField] private Transform playerCamera; // Main camera transform
        private Vector2 renderTextureSize; // Cached size of the render texture

        [Header("Camera Masks")]
        [SerializeField] private LayerMask mainCameraMask;
        [SerializeField] private LayerMask portalCameraMask;

        private IRoomWallsRegistry registry;
        private StateInputManager stateInputManager;
        [Inject]
        public void Construct(IRoomWallsRegistry registry, StateInputManager stateInputManager)
        {
            this.registry = registry;
            this.stateInputManager = stateInputManager;
        }

        [Header("Debug")]
        [SerializeField] private bool showDebugHUD = true;
        [SerializeField] private Color debugTextColor = Color.yellow;
        private GUIStyle debugStyle;
        private Vector2 debugOffset = new Vector2(610, 10);
        private float lineHeight = 20f;
        private Vector2 buttonSize = new Vector2(100, 40);

        // Animation parameter hashes
        private int isOpenHash = Animator.StringToHash("isOpen");
        private int isVisibleHash = Animator.StringToHash("isVisible");

        // Portal view variables
        private Material portalMaterial;
        private Vector3[] quadCorners = new Vector3[4];
        private Vector3[] screenSpaceCorners = new Vector3[4];
        private Vector2[] normalizedScreenSpaceCorners = new Vector2[4];
        private Mesh quadMesh;
        private Vector2[] originalUVs;

        private bool isRose = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Initialize camera masks
            if (mainCameraMask.value == 0)
            {
                mainCameraMask = LayerMask.GetMask("Player", "UI", "ActiveDoor");
            }

            if (portalCameraMask.value == 0)
            {
                portalCameraMask = LayerMask.GetMask("Player");
            }

            // Cache render texture size
            if (portalRenderTexture != null)
            {
                renderTextureSize = new Vector2(portalRenderTexture.width, portalRenderTexture.height);
            }

            // Set up portal view
            SetupPortalView();
        }

        void LateUpdate()
        {
            if (isActive && portalCamera != null && portalQuad != null)
            {
                UpdatePortalView();
            }
        }

        private void SetupPortalView()
        {
            if (portalCamera != null)
            {
                // Configure portal camera
                portalCamera.cullingMask = portalCameraMask;
                portalCamera.targetTexture = portalRenderTexture;
            }

            if (portalQuad != null)
            {
                // Get the material and mesh
                portalMaterial = portalQuad.material;
                quadMesh = portalQuad.GetComponent<MeshFilter>().mesh;

                // Store original UVs
                originalUVs = quadMesh.uv;

                // Get quad corners in local space
                List<Vector3> vertices = new List<Vector3>();
                quadMesh.GetVertices(vertices);
                quadCorners = vertices.ToArray();
            }

            // Set main camera culling mask if it exists
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.cullingMask = mainCameraMask;
            }
        }

        private void UpdatePortalView()
        {
            // Update portal camera position to match player camera
            if (playerCamera != null)
            {
                // Position the portal camera to match the main camera's relative position
                Vector3 playerOffsetFromPortal = playerCamera.position - transform.position;
                portalCamera.transform.position = transform.position + playerOffsetFromPortal;

                // Match rotation
                portalCamera.transform.rotation = playerCamera.rotation;
            }

            // Convert quad corners to screen space
            Camera mainCamera = Camera.main;
            if (screenSpaceCorners == null)
            {
                screenSpaceCorners = new Vector3[4];
            }
            if (mainCamera != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    // Transform local corners to world space
                    Vector3 worldCorner = portalQuad.transform.TransformPoint(quadCorners[i]);
                    // Convert to screen space
                    screenSpaceCorners[i] = mainCamera.WorldToScreenPoint(worldCorner);
                }
                for (int i = 0; i < 4; i++)
                {
                    normalizedScreenSpaceCorners[i].x = screenSpaceCorners[i].x / Screen.width;
                    normalizedScreenSpaceCorners[i].y = screenSpaceCorners[i].y / Screen.height;
                }

                // Calculate UV adjustments based on screen space coordinates
                UpdateQuadUVs();
            }
        }

        private void UpdateQuadUVs()
        {
            if (portalQuad == null || quadMesh == null) return;

            Vector2[] newUVs = new Vector2[quadMesh.uv.Length];

            // Map vertices to UVs in the correct order
            int[] vertexToUVMap = new int[] { 2, 3, 0, 1 }; // Maps vertex index to UV index

            // Update UVs
            for (int i = 0; i < newUVs.Length; i++)
            {
                int vertexIndex = vertexToUVMap[i];
                Vector2 normalizedPos = normalizedScreenSpaceCorners[i];

                newUVs[i] = new Vector2(normalizedPos.x, normalizedPos.y);
            }

            // Apply new UVs
            quadMesh.uv = newUVs;
            quadMesh.UploadMeshData(false);
        }

        /// <summary>
        /// Sets the portal to active or inactive state
        /// </summary>
        /// <param name="active">Whether the portal should be active</param>
        public void SetPortalActive(bool active)
        {
            isActive = active;

            // Update animator if available
            if (animator != null)
            {
                animator.SetBool(isOpenHash, active);
            }

            // Update particle effect if available
            if (portalEffect != null)
            {
                if (active)
                {
                    portalEffect.Play();
                }
                else
                {
                    portalEffect.Stop();
                }
            }

            if (doorCollider != null)
            {
                doorCollider.isTrigger = active;
            }

            // Enable/disable portal camera
            if (portalCamera != null)
            {
                portalCamera.enabled = active;
            }
        }

        public async void ActivatePortal()
        {
            await UniTask.WaitUntil(() => isRose);
            SetPortalActive(true);
        }

        /// <summary>
        /// Sets the visibility state in the animator
        /// </summary>
        /// <param name="visible">Whether the portal should be visible</param>
        public void SetVisibility(bool visible)
        {
            // Update animator if available
            if (animator != null)
            {
                animator.SetBool(isVisibleHash, visible);
                isRose = false;
            }

            // Enable/disable portal quad renderer
            if (portalQuad != null)
            {
                portalQuad.enabled = visible;
            }
        }

        /// <summary>
        /// Gets whether the portal is currently active
        /// </summary>
        public bool IsActive => isActive;

        /// <summary>
        /// Manually updates camera masks with the specified values
        /// </summary>
        /// <param name="newMainCameraMask">New layer mask for the main camera</param>
        /// <param name="newPortalCameraMask">New layer mask for the portal camera</param>
        public void UpdateCameraMasks(LayerMask newMainCameraMask, LayerMask newPortalCameraMask)
        {
            // Update stored values
            mainCameraMask = newMainCameraMask;
            portalCameraMask = newPortalCameraMask;

            // Apply to cameras
            if (portalCamera != null)
            {
                portalCamera.cullingMask = portalCameraMask;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.cullingMask = mainCameraMask;
            }

            Debug.Log("Camera masks manually updated");
        }

        /// <summary>
        /// Adjusts camera masks based on the level name
        /// </summary>
        /// <param name="levelName">The name of the loaded level</param>
        public void AdjustCameraMasksForLevelName(string levelName)
        {
            Debug.Log($"Adjusting camera masks for level: {levelName}");

            // Base masks always include these layers - build using bit operations
            int baseCameraMaskValue = 0;
            baseCameraMaskValue |= (1 << LayerMask.NameToLayer("Player"));
            baseCameraMaskValue |= (1 << LayerMask.NameToLayer("UI"));
            baseCameraMaskValue |= (1 << LayerMask.NameToLayer("ActiveDoor"));
            // No next room, but still show something in the portal
            baseCameraMaskValue |= (1 << LayerMask.NameToLayer("Effects"));

            int basePortalMaskValue = 0;
            basePortalMaskValue |= (1 << LayerMask.NameToLayer("Player"));

            // Current room's specific mask (to be added to main camera)
            int currentRoomMaskValue = 0;

            // Next room's specific mask (to be added to portal camera)
            int nextRoomMaskValue = 0;

            // Determine current room and what the next room would be
            if (levelName.Contains("Start") || levelName.Contains("Lobby") || levelName.Contains("MainMenu"))
            {
                // Start room - its specific mask
                currentRoomMaskValue |= (1 << LayerMask.NameToLayer("UI"));
                currentRoomMaskValue |= (1 << LayerMask.NameToLayer("Start"));
                // Next is Room 1
                nextRoomMaskValue |= (1 << LayerMask.NameToLayer("Room1"));
            }
            else if (levelName.Contains("Room1") || levelName == "1")
            {
                // Room 1 - its specific mask
                currentRoomMaskValue |= (1 << LayerMask.NameToLayer("Room1"));
                // Next is Room 2
                nextRoomMaskValue |= (1 << LayerMask.NameToLayer("Room2"));
            }
            else if (levelName.Contains("Room2") || levelName == "2")
            {
                // Room 2 - its specific mask
                currentRoomMaskValue |= (1 << LayerMask.NameToLayer("Room2"));
                // Next is Room 3
                nextRoomMaskValue |= (1 << LayerMask.NameToLayer("Room3"));
            }
            else if (levelName.Contains("Room3") || levelName == "3")
            {
                // Room 3 - its specific mask
                currentRoomMaskValue |= (1 << LayerMask.NameToLayer("Room3"));
                // Next is End Room
                nextRoomMaskValue |= (1 << LayerMask.NameToLayer("Room4"));
            }
            else if (levelName.Contains("End") || levelName.Contains("Ending"))
            {
                // End room - its specific mask
                currentRoomMaskValue |= (1 << LayerMask.NameToLayer("Room4"));
            }
            else
            {
                // Default - assume it's a generic room
                Debug.LogWarning($"Unknown room name: {levelName}, using default masks");
                currentRoomMaskValue |= (1 << LayerMask.NameToLayer("DefaultRoomElements"));
                nextRoomMaskValue = 0; // No next room elements
            }

            // Combine the base masks with the room-specific masks
            LayerMask newMainCameraMask = new LayerMask { value = baseCameraMaskValue | currentRoomMaskValue };
            LayerMask newPortalCameraMask = new LayerMask { value = basePortalMaskValue | nextRoomMaskValue };

            Debug.Log($"Main camera mask: Player + UI + ActiveDoor + {levelName} specific layers");
            Debug.Log($"Portal camera mask: Player + next room specific layers");

            // Apply the new masks
            UpdateCameraMasks(newMainCameraMask, newPortalCameraMask);
        }

        private void Rise()
        {
            isRose = true;
        }

        void OnGUI()
        {
            if (!showDebugHUD) return;

            if (debugStyle == null)
            {
                debugStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    normal = { textColor = debugTextColor }
                };
            }

            float yPos = debugOffset.y;
            float xPos = debugOffset.x;

            // Display screen space coordinates
            GUI.Label(new Rect(xPos, yPos, 500, lineHeight), "Portal Quad Screen Space Coordinates:", debugStyle);
            yPos += lineHeight;

            for (int i = 0; i < screenSpaceCorners.Length; i++)
            {
                string coordText = $"Vertex {i}: ({screenSpaceCorners[i].x:F1}, {screenSpaceCorners[i].y:F1})";
                GUI.Label(new Rect(xPos + 20, yPos, 500, lineHeight), coordText, debugStyle);
                yPos += lineHeight;
            }

            yPos += lineHeight;

            // Display UV coordinates side by side
            GUI.Label(new Rect(xPos, yPos, 500, lineHeight), "Portal Quad UV Coordinates (Original → Current):", debugStyle);
            yPos += lineHeight;

            if (quadMesh != null && originalUVs != null)
            {
                Vector2[] currentUVs = quadMesh.uv;
                for (int i = 0; i < currentUVs.Length; i++)
                {
                    string uvText = $"UV {i}: ({originalUVs[i].x:F3}, {originalUVs[i].y:F3}) → ({currentUVs[i].x:F3}, {currentUVs[i].y:F3})";
                    GUI.Label(new Rect(xPos + 20, yPos, 500, lineHeight), uvText, debugStyle);
                    yPos += lineHeight;
                }
            }

            // Display portal state
            yPos += lineHeight;
            GUI.Label(new Rect(xPos, yPos, 500, lineHeight), $"Portal Active: {isActive}", debugStyle);
            yPos += lineHeight;
            GUI.Label(new Rect(xPos, yPos, 500, lineHeight), $"Portal Visible: {portalQuad != null && portalQuad.enabled}", debugStyle);

            // Display screen dimensions for reference
            yPos += lineHeight * 2;
            GUI.Label(new Rect(xPos, yPos, 500, lineHeight), $"Screen Dimensions: {Screen.width}x{Screen.height}", debugStyle);

            yPos += lineHeight;
            if (GUI.Button(new Rect(xPos, yPos, buttonSize.x, buttonSize.y), "Open"))
            {
                stateInputManager.AddInput(new InputDoor(InputDoor.ActionType.Open));
            }
        }
    }
}
