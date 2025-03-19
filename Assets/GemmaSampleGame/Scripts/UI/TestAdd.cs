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
 * TestAdd.cs
 * 
 * Test utility for the chat panel user interface.
 * This component provides a debugging interface with buttons to test various
 * chat panel functions such as adding messages, clearing chat, showing/hiding
 * the thinking indicator, and controlling the panel visibility.
 * 
 * This class is primarily used during development for testing the chat UI
 * without requiring the full game flow.
 */
using UnityEngine;
using UnityEngine.UIElements;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    public class TestAdd : MonoBehaviour
    {
        /** Reference to the chat panel interface being tested */
        [SerializeField] private ChatPanelUserInterface chatPanelUserInterface;

        /** Test text to use for NPC messages */
        [SerializeField] private string npcText;

        /** Test text to use for player messages */
        [SerializeField] private string playerText;

        /** The UI Document containing the test controls */
        private UIDocument UIDocument;

        /** Root visual element of the UI Document */
        private VisualElement root;

        /** Button to add an NPC message to the chat */
        private Button addNpcButton;

        /** Button to add a player message to the chat */
        private Button addPlayerButton;

        /** Button to clear all chat messages */
        private Button clearButton;

        /** Button to show the thinking indicator */
        private Button thinkButton;

        /** Button to hide the thinking indicator */
        private Button doneThink;

        /** Button to open the chat panel */
        private Button open;

        /** Button to close the chat panel */
        private Button close;

        /** Button to scroll the chat to the bottom */
        private Button scroll;

        /**
         * Initializes the test interface by finding and registering all test buttons.
         * Sets up event handlers for each button to test different chat panel functions.
         */
        void Start()
        {
            UIDocument = GetComponent<UIDocument>();
            root = UIDocument.rootVisualElement;

            // Set up NPC message button
            addNpcButton = root.Q<Button>("add-npc-button");
            addNpcButton.clicked += () =>
            {
                chatPanelUserInterface.AddChat(new ChatPanelUserInterface.Chat(ChatPanelUserInterface.ChatType.Npc, npcText));
            };

            // Set up player message button
            addPlayerButton = root.Q<Button>("add-player-button");
            addPlayerButton.clicked += () =>
            {
                chatPanelUserInterface.AddChat(new ChatPanelUserInterface.Chat(ChatPanelUserInterface.ChatType.Player, playerText));
            };

            // Set up clear button
            clearButton = root.Q<Button>("clear");
            clearButton.clicked += () =>
            {
                chatPanelUserInterface.ClearChat();
            };

            // Set up thinking indicator buttons
            thinkButton = root.Q<Button>("think");
            thinkButton.clicked += () =>
            {
                chatPanelUserInterface.ShowThinking();
            };
            doneThink = root.Q<Button>("donethink");
            doneThink.clicked += () =>
            {
                chatPanelUserInterface.HideThinking();
            };

            // Set up panel open/close buttons
            open = root.Q<Button>("open");
            open.clicked += () =>
            {
                chatPanelUserInterface.Open();
            };
            close = root.Q<Button>("close");
            close.clicked += () =>
            {
                chatPanelUserInterface.Close();
            };

            // Set up scroll button
            scroll = root.Q<Button>("scroll");
            scroll.clicked += () =>
            {
                chatPanelUserInterface.ScrollToBottom();
            };
        }
    }
}