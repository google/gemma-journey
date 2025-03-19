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

namespace GoogleDeepMind.GemmaSampleGame
{
    [RequireComponent(typeof(UIDocument))]
    public class DialogueUIHandler : MonoBehaviour
    {
        [SerializeField]
        private UI.TextFieldData textFieldData;

        private VisualElement root;
        public TextField playerTextField;
        public Button submitButton;    // Submit message button
        public Button leaveButton;     // Leave conversation button
        private Label nameLabel;
        private Label playerTextLabel;
        private Label npcText;
        private ConversationManager conversationManager;

        [Inject]
        public void Construct(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;
            Debug.Log($"[DialogueUIHandler] Constructed with ConversationManager");
        }

        private void Awake()
        {
            var cm = this.conversationManager;
            root = GetComponent<UIDocument>().rootVisualElement;
            playerTextField = root.Q<TextField>("player-text-field");
            submitButton = root.Q<Button>("button-submit");
            leaveButton = root.Q<Button>("button-leave");
            nameLabel = root.Q<Label>("npc-name");
            playerTextLabel = root.Q<Label>("player-text-label");
            npcText = root.Q<Label>("npc-text");

            if (playerTextField != null)
            {
                Debug.Log($"[DialogueUIHandler] Found TextField, initial value: '{playerTextField.value}'");

                // Setup navigation submit handler
                playerTextField.RegisterCallback<NavigationSubmitEvent>(evt =>
                {
                    Debug.Log($"[DialogueUIHandler] TextField submitted via navigation. Action key? {evt.actionKey}");
                    SubmitCurrentText();
                });
            }
            else
            {
                Debug.LogError("[DialogueUIHandler] Could not find #player-text-field in UI Document");
            }

            if (submitButton != null)
            {
                submitButton.clicked += () =>
                {
                    Debug.Log("[DialogueUIHandler] Submit button clicked");
                    SubmitCurrentText();
                };
            }
            else
            {
                Debug.LogWarning("[DialogueUIHandler] Could not find #button-submit");
            }

            if (leaveButton != null)
            {
                leaveButton.clicked += () =>
                {
                    Debug.Log("[DialogueUIHandler] Leave button clicked");
                    conversationManager.LeaveConversation();
                };
            }
            else
            {
                Debug.LogWarning("[DialogueUIHandler] Could not find #button-leave");
            }

            // Log warnings for missing elements
            if (nameLabel == null) Debug.LogWarning("[DialogueUIHandler] Could not find #npc-name element in UI Document");
            if (playerTextLabel == null) Debug.LogWarning("[DialogueUIHandler] Could not find #player-text-label element in UI Document");
            if (npcText == null) Debug.LogWarning("[DialogueUIHandler] Could not find #npc-text element in UI Document");
            root.style.display = DisplayStyle.None; // Hide by default
        }

        private void OnDestroy()
        {
            if (textFieldData != null)
            {
                Debug.Log("[DialogueUIHandler] Cleaning up TextFieldData listeners");
                textFieldData.RemoveListener(value => playerTextField.value = value);
            }
        }

        public void Show()
        {
            if (playerTextField != null)
            {
                Debug.Log("[DialogueUIHandler] Showing and focusing TextField");
                playerTextField.Focus();
            }
        }

        public void Hide()
        {
            if (playerTextField != null && textFieldData != null)
            {
                Debug.Log("[DialogueUIHandler] Hiding and clearing TextField");
                textFieldData.SetValue(string.Empty);
                playerTextField.Blur();
            }
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                Debug.Log("[DialogueUIHandler] Enter key pressed, handling submit");
                HandleSubmit();
                evt.StopPropagation();
            }
        }

        private void HandleSubmit()
        {
            if (!string.IsNullOrWhiteSpace(textFieldData.Value))
            {
                string message = textFieldData.Value;
                Debug.Log($"[DialogueUIHandler] Submitting message: '{message}'");
                textFieldData.SetValue(string.Empty);
                conversationManager.SendMessage(message);
            }
            else
            {
                Debug.Log("[DialogueUIHandler] Ignoring empty message submit");
            }
        }

        private void SubmitCurrentText()
        {
            if (playerTextField != null)
            {
                string text = playerTextField.value;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    conversationManager.SendMessage(text);
                    playerTextField.value = string.Empty;
                }
                else
                {
                    Debug.Log("[DialogueUIHandler] Ignoring empty message submit");
                }
            }
        }

        public void SetInputEnabled(bool enabled)
        {
            if (playerTextField != null)
            {
                Debug.Log($"[DialogueUIHandler] Setting input elements {(enabled ? "enabled" : "disabled")}");

                // TextField
                playerTextField.SetEnabled(enabled);
                if (enabled)
                {
                    playerTextField.Focus();
                }
                else
                {
                    playerTextField.Blur();
                }

                // Buttons
                if (submitButton != null)
                {
                    submitButton.SetEnabled(enabled);
                }
                if (leaveButton != null)
                {
                    leaveButton.SetEnabled(enabled);
                }
            }
        }

        public void SetDisplayStyle(DisplayStyle displayStyle)
        {
            root.style.display = displayStyle;
        }

        public void SetNpcName(string name)
        {
            nameLabel.text = name;
        }
        public void SetNpcText(string text)
        {
            npcText.text = text;
        }

        public void SetPlayerText(string text)
        {
            playerTextLabel.text = text;
        }
    }
}