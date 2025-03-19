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
using UnityEngine.UIElements;
using VContainer;
using GemmaCpp;
using Cysharp.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class ChatbotForm : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private string npcName = "Gemma";

        // UI Elements
        private TextField playerTextInput;
        private Button submitButton;
        private Button leaveButton;
        private ScrollView chatContainer;
        private Label npcNameLabel;

        // Templates
        private VisualTreeAsset npcChatboxTemplate;
        private VisualTreeAsset playerChatboxTemplate;
        private VisualTreeAsset npcThinkingBoxTemplate;

        // Chat state
        private List<string> chatHistory = new List<string>();
        private bool isThinking = false;

        // Injected dependencies
        private GemmaManager gemmaManager;

        [Inject]
        public void Construct(GemmaManager gemmaManager)
        {
            Debug.Log("ChatbotForm: GemmaManager injected");
            this.gemmaManager = gemmaManager;
        }

        private void Start()
        {
            Debug.Log("ChatbotForm: Starting initialization");

            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
                if (uiDocument == null)
                {
                    Debug.LogError("ChatbotForm: UIDocument component not found");
                    return;
                }
            }

            // Get references to UI elements
            VisualElement root = uiDocument.rootVisualElement;

            playerTextInput = root.Q<TextField>("player-text-container");
            submitButton = root.Q<Button>("button-submit");
            leaveButton = root.Q<Button>("button-leave");
            chatContainer = root.Q<ScrollView>("chat-container");
            npcNameLabel = root.Q<Label>("npc-name");

            // Validate UI elements
            if (playerTextInput == null)
            {
                Debug.LogError("ChatbotForm: Player text input not found");
                return;
            }

            if (submitButton == null)
            {
                Debug.LogError("ChatbotForm: Submit button not found");
                return;
            }

            if (chatContainer == null)
            {
                Debug.LogError("ChatbotForm: Chat container not found");
                return;
            }

            // Set up UI
            if (npcNameLabel != null)
            {
                npcNameLabel.text = npcName;
            }

            // Load templates
            LoadTemplates();

            // Clear default template instances
            chatContainer.Clear();

            // Add initial NPC message
            AddNpcMessage("Hello! How can I help you today?");

            // Register event handlers
            submitButton.clicked += OnSubmitClicked;

            if (leaveButton != null)
            {
                leaveButton.clicked += () =>
                {
                    Debug.Log("ChatbotForm: Leave button clicked");
                    // Implement leave functionality if needed
                };
            }

            // Register enter key press on text field
            playerTextInput.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    if (!string.IsNullOrWhiteSpace(playerTextInput.value))
                    {
                        OnSubmitClicked();
                    }
                }
            });

            Debug.Log("ChatbotForm: Initialization complete");
        }

        private void LoadTemplates()
        {
            Debug.Log("ChatbotForm: Loading templates");

            // Load templates from Resources folder
            npcChatboxTemplate = Resources.Load<VisualTreeAsset>("NpcChatbox");
            playerChatboxTemplate = Resources.Load<VisualTreeAsset>("PlayerChatbox");
            npcThinkingBoxTemplate = Resources.Load<VisualTreeAsset>("NpcThinkingBox");

            if (npcChatboxTemplate == null)
            {
                Debug.LogWarning("ChatbotForm: NPC chatbox template not found in Resources");

                // Try to find it in the UXML
                var npcChatbox = chatContainer.Q("NpcChatbox");
                if (npcChatbox != null)
                {
                    Debug.Log("ChatbotForm: Found NPC chatbox in UXML, will use it as reference");
                }
            }
            else
            {
                Debug.Log("ChatbotForm: NPC chatbox template loaded from Resources");
            }

            if (playerChatboxTemplate == null)
            {
                Debug.LogWarning("ChatbotForm: Player chatbox template not found in Resources");

                // Try to find it in the UXML
                var playerChatbox = chatContainer.Q("PlayerChatbox");
                if (playerChatbox != null)
                {
                    Debug.Log("ChatbotForm: Found player chatbox in UXML, will use it as reference");
                }
            }
            else
            {
                Debug.Log("ChatbotForm: Player chatbox template loaded from Resources");
            }

            if (npcThinkingBoxTemplate == null)
            {
                Debug.LogWarning("ChatbotForm: NPC thinking box template not found in Resources");

                // Try to find it in the UXML
                var npcThinkingBox = chatContainer.Q("NpcThinkingBox");
                if (npcThinkingBox != null)
                {
                    Debug.Log("ChatbotForm: Found NPC thinking box in UXML, will use it as reference");
                }
            }
            else
            {
                Debug.Log("ChatbotForm: NPC thinking box template loaded from Resources");
            }
        }

        private void OnSubmitClicked()
        {
            if (isThinking)
            {
                Debug.Log("ChatbotForm: Already processing a request, ignoring");
                return;
            }

            string userInput = playerTextInput.value.Trim();
            if (string.IsNullOrEmpty(userInput))
            {
                Debug.Log("ChatbotForm: Empty input, not sending to Gemma");
                return;
            }

            Debug.Log($"ChatbotForm: Sending to Gemma: {userInput}");

            // Add player message to chat
            AddPlayerMessage(userInput);

            // Clear input field
            playerTextInput.value = "";

            // Show thinking indicator
            ShowThinkingIndicator();

            // Send to Gemma and get response
            SendToGemmaAsync(userInput).Forget();
        }

        private void AddNpcMessage(string message)
        {
            Debug.Log($"ChatbotForm: Adding NPC message: {TruncateForLogging(message)}");

            VisualElement npcChatbox;

            if (npcChatboxTemplate != null)
            {
                // Instantiate from template asset
                npcChatbox = npcChatboxTemplate.Instantiate();
                chatContainer.Add(npcChatbox);
            }
            else
            {
                // Create a simple fallback
                npcChatbox = CreateFallbackNpcMessage(message);
                chatContainer.Add(npcChatbox);
                return;
            }

            // Find and set the message text
            Label messageLabel = npcChatbox.Q<Label>("message-text");
            if (messageLabel != null)
            {
                messageLabel.text = message;
            }
            else
            {
                Debug.LogWarning("ChatbotForm: Could not find message-text in NpcChatbox template");

                // Try to find any Label in the template
                messageLabel = npcChatbox.Q<Label>();
                if (messageLabel != null)
                {
                    messageLabel.text = message;
                    Debug.Log("ChatbotForm: Found a Label element to use for the message");
                }
                else
                {
                    Debug.LogError("ChatbotForm: No Label found in NpcChatbox template");

                    // Add a label as a child
                    Label newLabel = new Label(message);
                    newLabel.AddToClassList("npc-message");
                    npcChatbox.Add(newLabel);
                }
            }

            // Scroll to bottom
            chatContainer.scrollOffset = new Vector2(0, chatContainer.contentContainer.worldBound.height);
        }

        private void AddPlayerMessage(string message)
        {
            Debug.Log($"ChatbotForm: Adding player message: {TruncateForLogging(message)}");

            VisualElement playerChatbox;

            if (playerChatboxTemplate != null)
            {
                // Instantiate from template asset
                playerChatbox = playerChatboxTemplate.Instantiate();
                chatContainer.Add(playerChatbox);
            }
            else
            {
                // Create a simple fallback
                playerChatbox = CreateFallbackPlayerMessage(message);
                chatContainer.Add(playerChatbox);
                return;
            }

            // Find and set the message text
            Label messageLabel = playerChatbox.Q<Label>("message-text");
            if (messageLabel != null)
            {
                messageLabel.text = message;
            }
            else
            {
                Debug.LogWarning("ChatbotForm: Could not find message-text in PlayerChatbox template");

                // Try to find any Label in the template
                messageLabel = playerChatbox.Q<Label>();
                if (messageLabel != null)
                {
                    messageLabel.text = message;
                    Debug.Log("ChatbotForm: Found a Label element to use for the message");
                }
                else
                {
                    Debug.LogError("ChatbotForm: No Label found in PlayerChatbox template");

                    // Add a label as a child
                    Label newLabel = new Label(message);
                    newLabel.AddToClassList("player-message");
                    playerChatbox.Add(newLabel);
                }
            }

            // Scroll to bottom
            chatContainer.scrollOffset = new Vector2(0, chatContainer.contentContainer.worldBound.height);
        }

        private void ShowThinkingIndicator()
        {
            Debug.Log("ChatbotForm: Showing thinking indicator");
            isThinking = true;

            VisualElement thinkingBox;

            if (npcThinkingBoxTemplate != null)
            {
                // Instantiate from template asset
                thinkingBox = npcThinkingBoxTemplate.Instantiate();
                thinkingBox.name = "thinking-indicator";
                chatContainer.Add(thinkingBox);
            }
            else
            {
                // Create a simple fallback
                thinkingBox = CreateFallbackThinkingIndicator();
                thinkingBox.name = "thinking-indicator";
                chatContainer.Add(thinkingBox);
            }

            // Scroll to bottom
            chatContainer.scrollOffset = new Vector2(0, chatContainer.contentContainer.worldBound.height);
        }

        private void HideThinkingIndicator()
        {
            Debug.Log("ChatbotForm: Hiding thinking indicator");
            isThinking = false;

            // Find and remove the thinking indicator
            VisualElement thinkingIndicator = chatContainer.Q(name: "thinking-indicator");
            if (thinkingIndicator != null)
            {
                chatContainer.Remove(thinkingIndicator);
            }
        }

        // Fallback UI creation methods
        private VisualElement CreateFallbackNpcMessage(string message)
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("npc-message-container");
            container.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            container.style.borderTopLeftRadius = 8;
            container.style.borderTopRightRadius = 8;
            container.style.borderBottomLeftRadius = 8;
            container.style.borderBottomRightRadius = 8;
            container.style.paddingTop = 10;
            container.style.paddingRight = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.marginBottom = 10;

            Label messageLabel = new Label(message);
            messageLabel.AddToClassList("npc-message");
            messageLabel.style.color = Color.white;
            container.Add(messageLabel);

            return container;
        }

        private VisualElement CreateFallbackPlayerMessage(string message)
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("player-message-container");
            container.style.backgroundColor = new Color(0.1f, 0.4f, 0.8f);
            container.style.borderTopLeftRadius = 8;
            container.style.borderTopRightRadius = 8;
            container.style.borderBottomLeftRadius = 8;
            container.style.borderBottomRightRadius = 8;
            container.style.paddingTop = 10;
            container.style.paddingRight = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.marginBottom = 10;
            container.style.alignSelf = Align.FlexEnd;

            Label messageLabel = new Label(message);
            messageLabel.AddToClassList("player-message");
            messageLabel.style.color = Color.white;
            container.Add(messageLabel);

            return container;
        }

        private VisualElement CreateFallbackThinkingIndicator()
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("thinking-indicator-container");
            container.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            container.style.borderTopLeftRadius = 8;
            container.style.borderTopRightRadius = 8;
            container.style.borderBottomLeftRadius = 8;
            container.style.borderBottomRightRadius = 8;
            container.style.paddingTop = 10;
            container.style.paddingRight = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.marginBottom = 10;

            Label thinkingLabel = new Label("Thinking...");
            thinkingLabel.AddToClassList("thinking-indicator");
            thinkingLabel.style.color = Color.white;
            container.Add(thinkingLabel);

            return container;
        }

        private async UniTaskVoid SendToGemmaAsync(string prompt)
        {
            try
            {
                if (gemmaManager == null)
                {
                    Debug.LogError("ChatbotForm: GemmaManager is null");
                    HideThinkingIndicator();
                    AddNpcMessage("Error: GemmaManager not available");
                    return;
                }

                Debug.Log("ChatbotForm: Starting Gemma generation");

                // Prepare for response
                StringBuilder responseBuilder = new StringBuilder();

                // Create a token callback to show streaming response
                Gemma.TokenCallback tokenCallback = (token) =>
                {
                    responseBuilder.Append(token);
                    return true;
                };

                // Generate response
                string response = await gemmaManager.GenerateResponseAsync(prompt, tokenCallback);

                Debug.Log($"ChatbotForm: Gemma response complete ({response.Length} chars)");

                // Hide thinking indicator and show response
                HideThinkingIndicator();
                AddNpcMessage(response);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ChatbotForm: Error generating response - {e.Message}");
                HideThinkingIndicator();
                AddNpcMessage("Sorry, I encountered an error while processing your request.");
            }
        }

        private string TruncateForLogging(string text, int maxLength = 50)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength) + "...";
        }
    }
}