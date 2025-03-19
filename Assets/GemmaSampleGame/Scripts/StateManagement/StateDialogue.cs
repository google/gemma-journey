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
 * StateDialogue.cs
 * 
 * Implements the dialogue state for the game state machine.
 * This state manages player interactions with NPCs, including
 * displaying the chat interface, handling player messages,
 * and processing responses from the NPC.
 * 
 * The dialogue state also handles transitions to and from other states
 * with appropriate camera blending and UI animations.
 */
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    [Serializable]
    public class StateDialogue : ClueGameState
    {
        /** Time delay (in seconds) before exiting the dialogue state */
        [SerializeField]
        private float exitTime = 2f;

        /** Reference to the camera brain for handling camera transitions */
        private CinemachineBrain brain;

        /** Timestamp when exit input was received (for delayed exit) */
        private float timeReceiveExitInput = -1;

        /** Manager for handling conversation flow and NPC interactions */
        [Inject]
        private ConversationManager _conversationManager;

        /** UI interface for displaying chat messages */
        [Inject]
        private UI.ChatPanelUserInterface _chatPanelUserInterface;

        /** Manager for level-related operations */
        [Inject]
        private LevelManager _levelManager;

        /** Whether the player has successfully completed the dialogue challenge */
        private bool _isPassed = false;

        /**
         * Called when entering the dialogue state.
         * Sets up the conversation with the appropriate NPC and initializes the chat UI.
         */
        protected override void InternalEnterState()
        {
            if (_inputManager.HasStateChangeInput<InputInteract>())
            {
                InputInteract input = _inputManager.GetLastStateChangeInput<InputInteract>();
                _conversationManager.SetActiveChat(input.InteractingAgent);
            }
            _chatPanelUserInterface.Open();

            if (brain == null)
            {
                brain = Camera.main.GetComponent<CinemachineBrain>();
            }
            timeReceiveExitInput = -1;
            _isPassed = false;
        }

        /**
         * Called when exiting the dialogue state.
         * Cleans up the conversation and closes the chat UI.
         */
        protected override void InternalExitState()
        {
            _conversationManager.SetActiveChat(null);
            _chatPanelUserInterface.Close();
        }

        /**
         * Updates the state each frame.
         * Handles timing for delayed exit from dialogue when interaction is complete.
         */
        public override void UpdateState()
        {
            if (_inputManager.HasInput<InputDoneInteract>())
            {
                timeReceiveExitInput = Time.time;
            }

            if (timeReceiveExitInput > 0 && Time.time - timeReceiveExitInput >= exitTime)
            {
                StateMachineChangeStateTo(_stateMachine.StateWalkAround);
                timeReceiveExitInput = 0;
            }
            base.UpdateState();
        }

        /**
         * Processes inputs specific to the dialogue state.
         * Handles player messages, conversation exit requests, and NPC responses.
         */
        protected override void InternalUpdateState()
        {
            if (_inputManager.HasInput<InputLeaveConversation>())
            {
                _conversationManager.LeaveConversation();
            }

            if (_inputManager.HasInput<InputPlayerMessage>())
            {
                string message = _inputManager.GetFirstInput<InputPlayerMessage>().Message;
                _conversationManager.SendMessage(message);
            }

            if (_inputManager.HasInput<InputNpcDone>())
            {
                _isPassed = true;
            }
        }

        /**
         * Checks if the transition into this state can be completed.
         * 
         * @return True if transition can be completed, false if still transitioning
         */
        protected override bool TransitionIn()
        {
            if ((brain != null && brain.IsBlending) || !_chatPanelUserInterface.IsReady)
            {
                return false;
            }
            return true;
        }

        /**
         * Called when the transition into this state is complete.
         */
        public override void TransitionInDone()
        {
            base.TransitionInDone();
        }

        /**
         * Checks if the transition out of this state can be completed.
         * 
         * @return True if transition can be completed, false if still transitioning
         */
        protected override bool TransitionOut()
        {
            if ((brain != null && brain.IsBlending) || !_chatPanelUserInterface.IsReady)
            {
                return false;
            }
            return true;
        }

        /**
         * Called when the transition out of this state is complete.
         * If the player has successfully passed the dialogue challenge,
         * opens the door to allow progression.
         */
        public override void TransitionOutDone()
        {
            base.TransitionOutDone();
            if (_isPassed)
            {
                _levelManager?.CurrentRoomManager?.DoorManager?.Open();
            }
        }
    }
}