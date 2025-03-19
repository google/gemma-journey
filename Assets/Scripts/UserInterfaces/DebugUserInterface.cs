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

using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    public class DebugUserInterface : UserInterface
    {
        private const string promptFolder = "prompts";
        private const string promptFilenameFormat = "{0}_{1}.txt";
        private const string dateTimeFormat = "yyyyMMdd_HHmmss";
        private Label nameText;
        private Label secretText;
        private Label statusText;
        private Label promptText;
        private Label responseText;

        private VisualElement container;
        private VisualElement opener;

        private Button openDoorButton;
        private Button resetConversationButton;
        private Button dumpConversationButton;

        private ConversationManager conversationManager;
        private SecretManager secretManager;
        private StateManagement.StateInputManager stateInputManager;

        [Inject]
        public void Construct(
            SecretManager secretManager,
            StateManagement.StateInputManager stateInputManager,
            ConversationManager conversationManager
        )
        {
            this.secretManager = secretManager;
            this.stateInputManager = stateInputManager;
            this.conversationManager = conversationManager;
        }

        protected override void Start()
        {
            nameText = root.Q<Label>("name-text");
            statusText = root.Q<Label>("status-text");
            secretText = root.Q<Label>("secret-text");
            promptText = root.Q<Label>("prompt-text");
            responseText = root.Q<Label>("response-text");
            openDoorButton = root.Q<Button>("open");
            openDoorButton.clicked += () =>
            {
                stateInputManager.AddInput(new StateManagement.InputDoor(StateManagement.InputDoor.ActionType.Open));
            };

            resetConversationButton = root.Q<Button>("reset-conversation");
            resetConversationButton.clicked += () =>
            {
                conversationManager.ResetConversation();
                if (conversationManager.ActiveChat != null)
                {
                    AddStatus($"Reset conversation called on {conversationManager.ActiveChat.DisplayName}\nClear chat history, chat panel and reload personality");
                }
            };

            dumpConversationButton = root.Q<Button>("dump-conversation");
            dumpConversationButton.clicked += () =>
            {
                if (conversationManager == null)
                {
                    AddStatus("Conversation manager is null");
                    return;
                }
                if (conversationManager.ActiveChat == null)
                {
                    AddStatus("Conversation manager: active chat is null");
                    return;
                }
                string filename = string.Format(promptFilenameFormat, conversationManager.ActiveChat.Name, DateTime.Now.ToString(dateTimeFormat));
                Debug.Log($"Filename: {filename}");
                string conversationDirectory = Path.Combine(Application.persistentDataPath, promptFolder);
                if (!Directory.Exists(conversationDirectory))
                {
                    Directory.CreateDirectory(conversationDirectory);
                }
                string savedConversationPath = Path.Combine(conversationDirectory, filename);
                string conversation = conversationManager.GetConversation();
                if (conversation != null)
                {
                    File.WriteAllText(savedConversationPath, conversation);
                    AddStatus($"Conversation with {conversationManager.ActiveChat.DisplayName} saved to: {savedConversationPath}");
                }
                else
                {
                    AddStatus("Conversation is null");
                }
            };

            container = root.Q<VisualElement>("container");
            opener = root.Q<VisualElement>("opener");
            opener.RegisterCallback<ClickEvent>((_) =>
            {
                container.ToggleInClassList("slide-open");
            });

            GemmaClient.OnPrompt += OnPrompt;
            GemmaClient.OnResponse += OnResponse;
            PersonalityProvider.OnPersonalityStatus += OnPersonalityStatus;
        }

        private void Update()
        {
            if (secretText != null && secretManager != null)
            {
                secretText.text = $"Current secrets: {secretManager.ToString()}";
            }

            if (nameText != null && conversationManager != null)
            {
                nameText.text = $"Active chat: {conversationManager.ActiveChat}";
            }
        }

        private void OnPrompt(GemmaClient client, string prompt)
        {
            if (promptText != null)
            {
                promptText.text = $"Prompt from {client.gameObject.name} :\n{prompt}";
            }
        }
        private void OnResponse(GemmaClient client, string response)
        {
            if (responseText != null)
            {
                responseText.text = $"Response from {client.gameObject.name}:\n{response}";
            }
        }

        private void OnPersonalityStatus(string message)
        {
            AddStatus(message);
        }

        private void AddStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text += message;
                statusText.text += "\n-----\n";
            }
        }
    }
}