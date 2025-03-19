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
 * ChatPanelUserInterface.cs
 * 
 * User interface for the in-game chat panel that displays conversations with NPCs.
 * Handles displaying player and NPC messages, typing indicators, scrolling, and input handling.
 * 
 * This class manages both the visual appearance of the chat panel and the interaction
 * logic, including sending messages, showing "thinking" animations, and transitioning
 * between UI states.
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    public class ChatPanelUserInterface : UserInterface
    {
        /** Text displayed when a conversation ends */
        public const string END_CONVERSATION_BLOCK = "End of conversation";

        /**
         * Types of chat messages that can be displayed in the panel
         */
        public enum ChatType
        {
            /** System message or separator (like "End of conversation") */
            Block,
            /** Message from the player */
            Player,
            /** Message from an NPC */
            Npc
        }

        /**
         * Data structure representing a single chat message
         */
        public class Chat
        {
            /** The type of message (Block, Player, or NPC) */
            public ChatType Type;

            /** The text content of the message */
            public string Content;

            /**
             * Creates a new chat message with the specified type and content
             * 
             * @param type The message type (Block, Player, or NPC)
             * @param content The text content of the message
             */
            public Chat(ChatType type, string content)
            {
                Type = type;
                Content = content;
            }
        }

        [Header("Chat panel parameters")]
        /** Template for NPC chat bubble UI elements */
        [SerializeField] private VisualTreeAsset npcChatTemplate;

        /** Template for player chat bubble UI elements */
        [SerializeField] private VisualTreeAsset playerChatTemplate;

        /** Sound played when the chat panel opens */
        [SerializeField] private AudioClip _openAudioClip;

        /** Sound played when the chat panel closes */
        [SerializeField] private AudioClip _closeAudioClip;

        /** ScrollView containing all chat messages */
        private ScrollView scrollView;

        /** Button to submit player messages */
        private Button submitButton;

        /** Button to leave the conversation */
        private Button leaveButton;

        /** Text input field for player messages */
        private TextField playerTextField;

        /** Label displaying the name of the NPC being conversed with */
        private Label npcName;

        /** Root element of the chat panel */
        private VisualElement chatPanel;

        /** Reference to the most recently added chat element */
        private VisualElement latestChatBox;

        /** First animated dot in the thinking indicator */
        private VisualElement firstDot;

        /** Second animated dot in the thinking indicator */
        private VisualElement secondDot;

        /** Third animated dot in the thinking indicator */
        private VisualElement thirdDot;

        /** Container for the animated typing indicator dots */
        private VisualElement dotContainer;

        /** Reference to the state input manager for sending state change events */
        private StateManagement.StateInputManager _stateInputManager;

        /** Holder for UI transition effects and animations */
        [SerializeField]
        private UI.TransitionHolder _transitionHolder;

        /** Whether the thinking animation has been started */
        private bool _startThinking = false;

        /**
         * Dependency injection method to receive required references.
         * 
         * @param stateInputManager Reference to the state input manager
         * @param transitionHolder Reference to the transition effect holder
         */
        [Inject]
        public void Construct
            (
            StateManagement.StateInputManager stateInputManager,
            UI.TransitionHolder transitionHolder
            )
        {
            _stateInputManager = stateInputManager;
            _transitionHolder = transitionHolder;
        }

        /**
         * Sets up UI elements and registers event callbacks when the UI is enabled.
         */
        protected override void OnEnable()
        {
            base.OnEnable();
            scrollView = root.Q<ScrollView>("chat-container");
            submitButton = root.Q<Button>("button-submit");
            submitButton.RegisterCallback<ClickEvent>(SubmitButton);
            leaveButton = root.Q<Button>("button-leave");
            leaveButton.RegisterCallback<ClickEvent>(LeaveConversation);
            npcName = root.Q<Label>("npc-name");
            playerTextField = root.Q<TextField>("player-text-container");
            playerTextField.RegisterCallback<KeyUpEvent>(EnterKeyUp);
            chatPanel = root.Q<VisualElement>("chat-panel");

            SetupDots();

            root.RegisterCallback<TransitionEndEvent>(HandleTransitionEnd);
            root.RegisterCallback<TransitionCancelEvent>(HandleTransitionCancel);
        }

        /**
         * Unregisters event callbacks when the UI is disabled.
         */
        protected override void OnDisable()
        {
            base.OnDisable();
            submitButton.UnregisterCallback<ClickEvent>(SubmitButton);
            leaveButton.UnregisterCallback<ClickEvent>(LeaveConversation);
            playerTextField.UnregisterCallback<KeyUpEvent>(EnterKeyUp);

            root.UnregisterCallback<TransitionEndEvent>(HandleTransitionEnd);
            root.UnregisterCallback<TransitionCancelEvent>(HandleTransitionCancel);
        }

        /**
         * Initializes the UI when starting.
         * Sets up transitions and starts in closed state.
         */
        protected override void Start()
        {
            CloseChatPanel();
            root.styleSheets.Add(_transitionHolder.StyleSheet);
            chatPanel.AddToClassList(_transitionHolder.TransitionLong);
        }

        /**
         * Opens the chat panel with a slide-in animation.
         * Focuses the text input field and plays an opening sound.
         */
        public override void Open()
        {
            isReady = false;
            chatPanel.RemoveFromClassList(_transitionHolder.SlideRightFade);
            if (playerTextField != null)
            {
                Debug.Log("[DialogueUIHandler] Showing and focusing TextField");
                playerTextField.Focus();
            }
            if (_openAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(_openAudioClip, transform.position);
            }
        }

        /**
         * Closes the chat panel with a slide-out animation.
         * Plays a closing sound effect.
         */
        public override void Close()
        {
            isReady = false;
            CloseChatPanel();
            if (_closeAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(_closeAudioClip, transform.position);
            }
        }

        /**
         * Internal method to handle the visual aspects of closing the chat panel.
         * Adds slide-out animation and blurs the text field.
         */
        private void CloseChatPanel()
        {
            chatPanel.AddToClassList(_transitionHolder.SlideRightFade);
            if (playerTextField != null)
            {
                Debug.Log("[DialogueUIHandler] Hiding and clearing TextField");
                playerTextField.Blur();
            }
        }

        /**
         * Adds multiple chat messages to the panel.
         * 
         * @param chats List of chat messages to add
         * @param clear Whether to clear existing messages first
         */
        public void AddChats(List<Chat> chats, bool clear = false)
        {
            if (clear)
            {
                ClearChat();
            }
            foreach (Chat chat in chats)
            {
                AddChat(chat);
            }
        }

        /**
         * Adds a single chat message to the panel.
         * 
         * @param chat The chat message to add
         */
        public void AddChat(Chat chat)
        {
            AddChat(chat.Type, chat.Content);
        }

        /**
         * Updates the content of the most recently added chat message.
         * Used for streaming AI responses as they're generated.
         * 
         * @param content The new content to display
         */
        public void EditChat(string content)
        {
            if (latestChatBox != null)
            {
                Label label = latestChatBox.Q<Label>("label");
                if (label != null)
                {
                    label.text = content;
                }
                ScrollToBottom();
            }
        }

        /**
         * Adds a new chat message of the specified type and content.
         * Creates the appropriate UI template based on message type.
         * 
         * @param chatType The type of message (Block, Player, or NPC)
         * @param content The text content of the message
         */
        public void AddChat(ChatType chatType, string content)
        {
            TemplateContainer templateContainer = null;
            switch (chatType)
            {
                case ChatType.Block:
                    break;
                case ChatType.Player:
                    {
                        Debug.Log("Add Player");
                        templateContainer = playerChatTemplate.Instantiate();
                        Label label = templateContainer.Q<Label>("label");
                        label.text = content;
                        break;
                    }
                case ChatType.Npc:
                    {
                        Debug.Log("Add Npc");
                        templateContainer = npcChatTemplate.Instantiate();
                        Label label = templateContainer.Q<Label>("label");
                        label.text = content;
                        break;
                    }
            }
            if (templateContainer != null)
            {
                scrollView.Insert(Mathf.Max(0, scrollView.childCount - 1), templateContainer);
                ScrollToBottom();
                latestChatBox = templateContainer;
            }
        }

        /**
         * Handles the leave button click event by sending a leave conversation input.
         * 
         * @param click The click event data
         */
        private void LeaveConversation(ClickEvent click)
        {
            _stateInputManager.AddInput(new StateManagement.InputLeaveConversation());
        }

        /**
         * Handles key press events in the text input field.
         * Submits the message when Enter key is pressed.
         * 
         * @param keyUpEvent The key event data
         */
        private void EnterKeyUp(KeyUpEvent keyUpEvent)
        {
            if (keyUpEvent.keyCode == KeyCode.Return)
            {
                SubmitCurrentText();
            }
        }

        /**
         * Handles the submit button click event.
         * 
         * @param buttonEvent The click event data
         */
        private void SubmitButton(ClickEvent buttonEvent)
        {
            SubmitCurrentText();
        }

        /**
         * Submits the current text in the input field if it's not empty.
         * Clears the input field after submission.
         */
        private void SubmitCurrentText()
        {
            if (playerTextField != null)
            {
                string text = playerTextField.value;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    _stateInputManager.AddInput(new StateManagement.InputPlayerMessage(text));
                    playerTextField.value = string.Empty;
                }
                else
                {
                    Debug.Log("[DialogueUIHandler] Ignoring empty message submit");
                }
            }
        }

        /**
         * Scrolls the chat panel to the bottom to show newest messages.
         * Uses UniTask to wait for layout updates before scrolling.
         */
        public async void ScrollToBottom()
        {
            await UniTask.WaitForEndOfFrame();
            scrollView.verticalScroller.value = scrollView.verticalScroller.highValue > 0 ? scrollView.verticalScroller.highValue : 0;
        }

        /**
         * Enables or disables the input controls (text field, submit and leave buttons).
         * 
         * @param enabled Whether input controls should be enabled
         */
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

        /**
         * Sets the display name of the NPC in the chat panel.
         * 
         * @param name The name to display
         */
        public void SetNpcName(string name)
        {
            npcName.text = name;
        }

        /**
         * Clears all messages from the chat panel.
         */
        public void ClearChat()
        {
            int count = scrollView.childCount - 1;
            for (int i = 0; i < count; i++)
            {
                scrollView.RemoveAt(0);
            }
        }

        /**
         * Handles transition end events by marking the UI as ready.
         * 
         * @param transitionEndEvent The transition event data
         */
        private void HandleTransitionEnd(TransitionEndEvent transitionEndEvent)
        {
            isReady = true;
        }

        /**
         * Handles transition cancel events by marking the UI as ready.
         * 
         * @param transitionCancelEvent The transition cancel event data
         */
        private void HandleTransitionCancel(TransitionCancelEvent transitionCancelEvent)
        {
            isReady = true;
        }

        /**
         * Shows the typing indicator animation when the NPC is thinking.
         */
        public void ShowThinking()
        {
            if (dotContainer.ClassListContains("slide-thinking"))
            {
                dotContainer.RemoveFromClassList("slide-thinking");
            }
            if (!_startThinking)
            {
                firstDot.AddToClassList("fade-dot");
                secondDot.AddToClassList("fade-dot");
                thirdDot.AddToClassList("fade-dot");
                _startThinking = true;
            }
        }

        /**
         * Hides the typing indicator animation when the NPC is done thinking.
         */
        public void HideThinking()
        {
            if (!dotContainer.ClassListContains("slide-thinking"))
            {
                dotContainer.AddToClassList("slide-thinking");
            }
            ScrollToBottom();
        }

        /**
         * Sets up the animated typing indicator dots with appropriate transitions.
         */
        private void SetupDots()
        {
            dotContainer = root.Q<VisualElement>("dot-container");
            dotContainer.RegisterCallback<TransitionEndEvent>((evt) =>
            {
                ScrollToBottom();
            });
            dotContainer.RegisterCallback<TransitionCancelEvent>((evt) =>
            {
                ScrollToBottom();
            });
            firstDot = root.Q<VisualElement>("first-dot");
            firstDot.RegisterCallback<TransitionEndEvent>((evt) =>
            {
                firstDot.ToggleInClassList("fade-dot");
            });
            secondDot = root.Q<VisualElement>("second-dot");
            secondDot.RegisterCallback<TransitionEndEvent>((evt) =>
            {
                secondDot.ToggleInClassList("fade-dot");
            });
            thirdDot = root.Q<VisualElement>("third-dot");
            thirdDot.RegisterCallback<TransitionEndEvent>((evt) =>
            {
                thirdDot.ToggleInClassList("fade-dot");
            });
        }

        /**
         * Sets up a single dot in the typing indicator.
         * 
         * @param dot The dot visual element
         * @param delayMs The animation delay in milliseconds
         */
        private void SetupDot(VisualElement dot, long delayMs)
        {
            dot.RegisterCallback<TransitionEndEvent>((evt) =>
            {
                dot.ToggleInClassList("fade-dot");
            });
        }
    }
}