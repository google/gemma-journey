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

using VContainer;
using VContainer.Unity;
using UnityEngine;
using GemmaCpp;
using UnityEngine.UIElements;
using System.Linq;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class GameLifetimeScope : LifetimeScope, IStateMachineViewHolder
    {
        [SerializeField]
        private GameSettings gameSettings;
        [SerializeField]
        private NPCGlobalSettings npcGlobalSettings;
        [SerializeField]
        private GemmaManagerSettings gemmaSettings;
        [SerializeField]
        private UserInterfaceManager userInterfaceManager;
        [SerializeField]
        private StateManagement.GameStateMachine gameStateMachine;
        [SerializeField]
        private LevelManager levelManager;
        [SerializeField] private PortalManager portalManager;
        [SerializeField]
        private PlayerManager playerManager;

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
        private Vector2 debugHUDPosition = new Vector2(10, 10);
        [SerializeField]
        private Vector2 debugHUDSize = new Vector2(300, 200);
        [SerializeField]
        private KeyCode toggleDebugHUDKey = KeyCode.F1;

        // Style for the debug text and background
        private GUIStyle debugTextStyle;
        private GUIStyle debugBackgroundStyle;

        protected override void Configure(IContainerBuilder builder)
        {
            // Register settings
            builder.RegisterInstance(gameSettings);
            builder.RegisterInstance(npcGlobalSettings);

            // Create and configure GemmaManager
            var gemmaManagerObj = new GameObject("GemmaManager");
            var gemmaManager = gemmaManagerObj.AddComponent<GemmaManager>();
            gemmaManager.settings = gemmaSettings;
            DontDestroyOnLoad(gemmaManagerObj);

            // Create ConversationManager
            var conversationManager = new ConversationManager();

            // Create SecretManager
            var secretManager = new SecretManager();

            // Register user interfaces
            userInterfaceManager.RegisterUserInterface(builder);

            var roomWallsRegistry = new RoomWallsRegistry();

            // Register instances
            builder.RegisterInstance(gameStateMachine);
            builder.RegisterInstance(gameStateMachine.InputManager);
            builder.RegisterInstance(gemmaManager);
            builder.RegisterInstance(conversationManager);
            builder.RegisterInstance(secretManager);
            builder.RegisterInstance(levelManager);
            builder.RegisterInstance(portalManager);
            builder.RegisterInstance(roomWallsRegistry).As<IRoomWallsRegistry>();
            // Register the player manager
            builder.RegisterInstance(playerManager);

            levelManager.RegisterLifetimeScope(this);
            playerManager.RegisterLifetimeScope(this);

            // Register all spawn points in the scene
            // builder.RegisterComponentInHierarchy<SpawnPoint>();

            // Register the spawn point tester
            /*
            builder.RegisterComponentInHierarchy<SpawnPointTester>();

            // Register the player controller
            var player = GameObject.FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError("Could not find PlayerController in scene!");
            }
            builder.RegisterInstance(player);
            */
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
            Container.Inject(levelManager);
            Container.Inject(portalManager);
            Container.Inject(playerManager);
            userInterfaceManager.InitUserInterface(Container);

            gameStateMachine.InitStateMachine(this);

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
            levelManager.Init();
            gameStateMachine.StartMachine();
        }

        private void Update()
        {
            gameStateMachine.UpdateStateMachine();

            // Toggle debug HUD with key press
            if (Input.GetKeyDown(toggleDebugHUDKey))
            {
                showDebugHUD = !showDebugHUD;
            }
        }

        private void OnGUI()
        {
            if (!showDebugHUD) return;

            // Draw background
            GUI.Box(new Rect(debugHUDPosition.x, debugHUDPosition.y, debugHUDSize.x, debugHUDSize.y), "", debugBackgroundStyle);

            // Get current state information
            var currentState = gameStateMachine.CurrentState;
            var destinationState = gameStateMachine.DestinationState;

            // Build debug information string
            string debugInfo = "<b>STATE MACHINE DEBUG</b>\n\n";

            if (currentState != null)
            {
                debugInfo += $"<b>Current State:</b> {currentState.GetType().Name}\n";

                // If the current state is a StateWithTransition, show its update step
                if (currentState is StateManagement.StateWithTransition<StateManagement.GameStateMachine, GameLifetimeScope> stateWithTransition)
                {
                    debugInfo += $"<b>Update Step:</b> {stateWithTransition.CurrentUpdateStep}\n";
                }

                // Show available transitions
                debugInfo += "\n<b>Available Transitions:</b> ";
                var destinations = currentState.GetDestinations();
                if (destinations != null && destinations.Count > 0)
                {
                    debugInfo += string.Join(", ", destinations.Select(s => s.GetType().Name));
                }
                else
                {
                    debugInfo += "None";
                }
                debugInfo += "\n";
            }
            else
            {
                debugInfo += "<b>Current State:</b> None\n";
            }

            if (destinationState != null)
            {
                debugInfo += $"\n<b>Destination State:</b> {destinationState.GetType().Name}\n";
            }
            else
            {
                debugInfo += "\n<b>Destination State:</b> None\n";
            }

            // Show recent inputs (if any)
            debugInfo += "\n<b>Recent Inputs:</b>\n";
            var inputs = gameStateMachine.InputManager.GetInputs();
            if (inputs != null && inputs.Count > 0)
            {
                foreach (var input in inputs.Take(5)) // Show up to 5 recent inputs
                {
                    debugInfo += $"- {input.GetType().Name}\n";
                }
            }
            else
            {
                debugInfo += "None\n";
            }

            // Add frame rate info
            debugInfo += $"\n<b>FPS:</b> {Mathf.Round(1.0f / Time.smoothDeltaTime)}\n";

            // Add toggle hint
            debugInfo += $"\n<i>Press {toggleDebugHUDKey} to toggle this display</i>";

            // Display the debug information
            debugTextStyle.richText = true;
            GUI.Label(new Rect(debugHUDPosition.x, debugHUDPosition.y, debugHUDSize.x, debugHUDSize.y), debugInfo, debugTextStyle);
        }
    }
}