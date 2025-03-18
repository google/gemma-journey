using UnityEngine;
using VContainer;
using System;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class PortalManager : MonoBehaviour
    {
        [SerializeField] private GameObject portalDoor;
        private PortalDoor portalDoorComponent;
        [SerializeField] private GameObject portalDoorNextRoom;
        private PortalDoor portalDoorNextRoomComponent;
        [SerializeField] private bool isVisible = false;
        [SerializeField] private float portalActivationDelay = 1.0f;

        [Header("Debug")]
        [SerializeField] private bool showDebugHUD = true;
        [SerializeField] private Color debugTextColor = Color.green;
        [SerializeField] private Vector2 debugOffset = new Vector2(10, 60);
        private GUIStyle debugStyle;
        private float lineHeight = 20f;
        private string currentRoomName = "";
        private string nextRoomName = "";

        private LevelManager levelManager;
        private StateManagement.StateInputManager stateInputManager;

        // Event that fires when the portal is activated
        public event Action OnPortalActivated;

        [Inject]
        public void Construct(
            LevelManager levelManager,
            StateManagement.StateInputManager stateInputManager)
        {
            this.levelManager = levelManager;
            this.stateInputManager = stateInputManager;
        }

        private void Start()
        {
            // Use the existing door GameObject
            if (portalDoor != null)
            {
                portalDoorComponent = portalDoor.GetComponent<PortalDoor>();
                if (portalDoorComponent == null)
                {
                    Debug.LogWarning("PortalDoor component not found on the provided door GameObject. Can't continue.");
                }
                else
                {
                    // Initially set visibility
                    SetPortalVisibility(isVisible);

                    // Position the portal at the current room's back anchor
                    PositionPortalAtCurrentRoom();

                    // Start listening for level loaded events
                    if (stateInputManager != null)
                    {
                        Debug.Log("Setting up level load listener");
                        StartLevelChangeListener();
                    }
                }
            }
            else
            {
                Debug.LogError("Portal Door GameObject is not assigned in the PortalManager!");
            }
        }

        private void OnDestroy()
        {
            // Clean up listeners
            StopLevelChangeListener();
        }

        private void StartLevelChangeListener()
        {
            // Check for level loaded events in the Update method
            Debug.Log("Level change listener started");
        }

        private void StopLevelChangeListener()
        {
            Debug.Log("Level change listener stopped");
        }

        private void Update()
        {
            // Always update room names for debug HUD if enabled
            if (showDebugHUD && levelManager != null && levelManager.CurrentRoomManager != null)
            {
                currentRoomName = GetCurrentLevelName();
                nextRoomName = GetNextLevelName();
            }

            // Check for portal-related input events
            if (stateInputManager != null)
            {
                // Door/portal interaction
                if (stateInputManager.HasInput<StateManagement.InputDoor>())
                {
                    var input = stateInputManager.GetLastInput<StateManagement.InputDoor>();
                    if (input.Type == StateManagement.InputDoor.ActionType.Enter && isVisible)
                    {
                        // Player has entered the door/portal area
                        ActivatePortal();
                    }
                }

                // Level change events
                if (stateInputManager.HasInput<StateManagement.InputLevelLoaded>())
                {
                    var input = stateInputManager.GetLastInput<StateManagement.InputLevelLoaded>();
                    Debug.Log($"Level loaded event detected: {input.LevelType}");

                    // Update portal door camera masks
                    if (portalDoorComponent != null && levelManager != null)
                    {
                        // Get both current and next level names
                        currentRoomName = GetCurrentLevelName();
                        nextRoomName = GetNextLevelName();

                        if (!string.IsNullOrEmpty(currentRoomName))
                        {
                            Debug.Log($"Detected current room: {currentRoomName}");

                            // Get custom mask configuration for this room sequence
                            UpdatePortalMasksForRoomSequence();
                        }
                        else
                        {
                            Debug.LogWarning("Could not determine current level name");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the name of the current level from the level manager
        /// </summary>
        /// <returns>The name of the current level, or empty string if not available</returns>
        private string GetCurrentLevelName()
        {
            if (levelManager == null || levelManager.CurrentRoomManager == null)
            {
                return string.Empty;
            }

            // Get the name of the room from the room manager
            string roomName = levelManager.CurrentRoomManager.name;

            // Clean up the name to extract just the room identifier

            // Check for specific room name patterns
            if (roomName.Contains("Start") || roomName.Contains("Lobby") || roomName.Contains("MainMenu"))
            {
                return "Start";
            }

            if (roomName.Contains("End") || roomName.Contains("Finish"))
            {
                return "End";
            }

            // Check for numbered rooms
            if (roomName.Contains("Room"))
            {
                // Try to extract the room number
                for (int i = 1; i <= 3; i++)
                {
                    if (roomName.Contains($"Room{i}") || roomName.Contains($"Room {i}"))
                    {
                        return $"Room{i}";
                    }
                }
            }

            // Just a number?
            if (roomName == "1" || roomName == "2" || roomName == "3")
            {
                return $"Room{roomName}";
            }

            // If it's a scene name with path, extract just the name
            if (roomName.Contains("/"))
            {
                roomName = System.IO.Path.GetFileNameWithoutExtension(roomName);
            }

            return roomName;
        }

        /// <summary>
        /// Gets the name of the next level based on the current level
        /// </summary>
        /// <returns>The name of the next level</returns>
        private string GetNextLevelName()
        {
            string currentName = GetCurrentLevelName();

            // Determine the next room in sequence
            if (currentName == "Start")
            {
                return "Room1";
            }
            else if (currentName == "Room1")
            {
                return "Room2";
            }
            else if (currentName == "Room2")
            {
                return "Room3";
            }
            else if (currentName == "Room3")
            {
                return "End";
            }
            else if (currentName == "End")
            {
                return "Start"; // Loop back to start
            }

            // If we can't determine, default to the next room being Room1
            return "Room1";
        }

        /// <summary>
        /// Called when a player enters the portal trigger area
        /// This is called by the PortalDoor component using SendMessageUpwards
        /// </summary>
        public void OnPlayerEnteredPortal()
        {
            if (isVisible)
            {
                Debug.Log("Player entered portal - activating portal");
                ActivatePortal();
            }
        }

        /// <summary>
        /// Positions the portal door at the back anchor of the current room
        /// </summary>
        public void PositionPortalAtCurrentRoom()
        {
            if (portalDoor == null)
            {
                Debug.Log("PositionPortalAtCurrentRoom(): No door to move.");
                return;
            }
            if (levelManager == null)
            {
                Debug.Log("PositionPortalAtCurrentRoom(): No level manager.");
                return;
            }
            else if (levelManager.CurrentRoomManager == null)
            {
                Debug.Log("PositionPortalAtCurrentRoom(): No room manager.");
                return;
            }

            Transform backAnchor = levelManager.CurrentRoomManager.BackAnchor;
            if (backAnchor != null)
            {
                portalDoor.transform.position = backAnchor.position;
                portalDoor.transform.rotation = backAnchor.rotation;
                Debug.Log($"Portal positioned at back anchor of room: {levelManager.CurrentRoomManager.name}");
            }
            else
            {
                Debug.LogWarning("Back anchor not found in the current room!");
            }
        }

        /// <summary>
        /// Sets the visibility of the portal door
        /// </summary>
        /// <param name="visible">Whether the portal should be visible</param>
        public void SetPortalVisibility(bool visible)
        {
            isVisible = visible;

            if (portalDoor != null)
            {
                // Set the GameObject active state
                portalDoor.SetActive(visible);

                // If the portal has a PortalDoor component, update its state
                if (portalDoorComponent != null)
                {
                    // Set the active state for effects
                    portalDoorComponent.SetPortalActive(visible);

                    // Set the visibility parameter in the animator
                    portalDoorComponent.SetVisibility(visible);
                }

                Debug.Log($"Portal visibility set to: {visible}");
            }
        }

        /// <summary>
        /// Activates the portal, triggering the transition to the next room
        /// </summary>
        public async void ActivatePortal()
        {
            if (portalDoorComponent != null && isVisible)
            {
                Debug.Log("Portal activated - preparing for room transition");

                // Activate the portal visual effects
                portalDoorComponent.SetPortalActive(true);

                // Invoke the portal activated event
                OnPortalActivated?.Invoke();

                // Wait for the activation delay
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(portalActivationDelay));

                // Trigger the level change in the LevelManager
                if (levelManager != null)
                {
                    // After changing the level, reposition the portal at the new room's back anchor
                    PositionPortalAtCurrentRoom();
                }
            }
        }

        /// <summary>
        /// Gets the current portal door GameObject
        /// </summary>
        public GameObject PortalDoorObject => portalDoor;

        /// <summary>
        /// Gets whether the portal is currently visible
        /// </summary>
        public bool IsVisible => isVisible;

        /// <summary>
        /// Updates the camera masks on the portal door
        /// </summary>
        /// <param name="mainCameraMask">New layer mask for the main camera</param>
        /// <param name="portalCameraMask">New layer mask for the portal camera</param>
        public void UpdatePortalCameraMasks(LayerMask mainCameraMask, LayerMask portalCameraMask)
        {
            if (portalDoorComponent != null)
            {
                portalDoorComponent.UpdateCameraMasks(mainCameraMask, portalCameraMask);
                Debug.Log("Portal camera masks updated from PortalManager");
            }
            else
            {
                Debug.LogWarning("Cannot update portal camera masks - no portal door component found");
            }
        }

        /// <summary>
        /// Updates the camera masks based on the specified level name
        /// </summary>
        /// <param name="levelName">The level name to adjust masks for</param>
        public void UpdatePortalCameraMasksForLevel(string levelName)
        {
            if (portalDoorComponent != null)
            {
                portalDoorComponent.AdjustCameraMasksForLevelName(levelName);
                Debug.Log($"Portal camera masks updated for level: {levelName}");
            }
            else
            {
                Debug.LogWarning("Cannot update portal camera masks - no portal door component found");
            }
        }

        /// <summary>
        /// Updates the portal masks based on the current room and next room
        /// </summary>
        private void UpdatePortalMasksForRoomSequence()
        {
            string currentRoom = GetCurrentLevelName();
            string nextRoom = GetNextLevelName();

            Debug.Log($"Updating camera masks for room sequence: {currentRoom} → {nextRoom}");

            // Base masks are always included - precompute using bit operations for performance
            int baseCameraMaskValue = 0;
            baseCameraMaskValue |= (1 << LayerMask.NameToLayer("Player"));
            baseCameraMaskValue |= (1 << LayerMask.NameToLayer("UI"));
            baseCameraMaskValue |= (1 << LayerMask.NameToLayer("ActiveDoor"));
            baseCameraMaskValue |= (1 << LayerMask.NameToLayer("Effects"));

            int basePortalMaskValue = 0;
            basePortalMaskValue |= (1 << LayerMask.NameToLayer("Player"));

            // Current room's specific mask elements
            LayerMask currentRoomMask = GetLayerMaskForRoom(currentRoom);

            // Next room's specific mask elements
            LayerMask nextRoomMask = GetLayerMaskForRoom(nextRoom);

            // Combine masks using bitwise operations
            LayerMask mainCameraMask = new LayerMask { value = baseCameraMaskValue | currentRoomMask.value };
            LayerMask portalCameraMask = new LayerMask { value = basePortalMaskValue | nextRoomMask.value };

            // Apply the masks
            UpdatePortalCameraMasks(mainCameraMask, portalCameraMask);
        }

        /// <summary>
        /// Gets the specific layer mask for a given room
        /// </summary>
        /// <param name="roomName">The name of the room</param>
        /// <returns>A LayerMask for the specific elements in that room</returns>
        private LayerMask GetLayerMaskForRoom(string roomName)
        {
            // Use cached layer values instead of calling GetMask at runtime
            int layerValue = 0;

            switch (roomName)
            {
                case "Start":
                    // UI layer (5) and StartRoomElements (e.g., 15)
                    layerValue |= (1 << 5); // UI
                    layerValue |= (1 << LayerMask.NameToLayer("Start"));
                    break;

                case "Room1":
                    layerValue |= (1 << LayerMask.NameToLayer("Room1"));
                    break;

                case "Room2":
                    layerValue |= (1 << LayerMask.NameToLayer("Room2"));
                    break;

                case "Room3":
                    layerValue |= (1 << LayerMask.NameToLayer("Room3"));
                    break;

                case "End":
                    layerValue |= (1 << LayerMask.NameToLayer("Room4"));
                    break;

                default:
                    // For unknown rooms, use a default
                    layerValue |= (1 << LayerMask.NameToLayer("Default"));
                    break;
            }

            return new LayerMask { value = layerValue };
        }

        /// <summary>
        /// Draws debug HUD information about room navigation
        /// </summary>
        private void OnGUI()
        {
            if (!showDebugHUD) return;

            if (debugStyle == null)
            {
                debugStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = debugTextColor }
                };
            }

            float yPos = debugOffset.y;
            float xPos = debugOffset.x;

            // Display level info
            GUI.Label(new Rect(xPos, yPos, 300, lineHeight), "Level Navigation Info:", debugStyle);
            yPos += lineHeight;

            GUI.Label(new Rect(xPos + 20, yPos, 300, lineHeight), $"Current Room: {currentRoomName}", debugStyle);
            yPos += lineHeight;

            GUI.Label(new Rect(xPos + 20, yPos, 300, lineHeight), $"Next Room: {nextRoomName}", debugStyle);
            yPos += lineHeight;

            GUI.Label(new Rect(xPos + 20, yPos, 300, lineHeight), $"Portal Active: {isVisible}", debugStyle);
            yPos += lineHeight;

            // Display info about level manager status
            if (levelManager != null)
            {
                yPos += lineHeight;
                GUI.Label(new Rect(xPos, yPos, 300, lineHeight), "Level Manager Status:", debugStyle);
                yPos += lineHeight;

                RoomManager currentRM = levelManager.CurrentRoomManager;
                GUI.Label(new Rect(xPos + 20, yPos, 300, lineHeight),
                    $"Room Manager: {(currentRM != null ? currentRM.name : "None")}", debugStyle);
            }
            else
            {
                yPos += lineHeight;
                GUI.Label(new Rect(xPos, yPos, 300, lineHeight), "Level Manager: Not Assigned", debugStyle);
            }
        }
    }
}