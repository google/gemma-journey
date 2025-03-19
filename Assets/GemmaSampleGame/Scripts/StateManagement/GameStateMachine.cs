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
 * GameStateMachine.cs
 * 
 * Main state machine that controls the overall game flow for the Clue demo.
 * Inherits from StateMachineWithTransition and manages transitions between
 * different game states like menu, tutorial, dialogue, etc.
 */
using UnityEngine;
using UnityEngine.Events;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public partial class GameStateMachine : StateMachineWithTransition<GameStateMachine, GameLifetimeScope>
    {
        [SerializeField]
        private UnityEvent<ClueGameState> StateChanged;

        /**
         * Initializes and starts the state machine with the starting state.
         * Called when the game begins.
         */
        public override void StartMachine()
        {
            _currentState = StateStart;
            _currentState.EnterState();
        }

        /**
         * Stops the state machine.
         * Currently not implemented but could be used for cleanup.
         */
        public override void StopMachine()
        {
            // Not currently implemented
        }

        /**
         * Updates the state machine each frame.
         * Handles global inputs like sound toggle and quit commands.
         */
        public override void UpdateStateMachine()
        {
            base.UpdateStateMachine();

            // Handle sound toggle input
            if (InputManager.HasInput<InputSound>())
            {
                Debug.Log("Toggle sound");
                bool enabled = InputManager.GetFirstInput<InputSound>().Enabled;
                if (enabled)
                {
                    AudioListener.pause = false;
                    AudioListener.volume = 1;
                }
                else
                {
                    AudioListener.pause = true;
                    AudioListener.volume = 0;
                }
            }

            // Handle quit game input
            if (InputManager.HasInput<InputQuit>())
            {
                Debug.Log("Quit game");
                Application.Quit();
            }
        }

        /**
         * Called when a state is successfully entered.
         * Broadcasts the state change event to listeners.
         */
        public override void StateSuccessfullyEnter()
        {
            base.StateSuccessfullyEnter();
            StateChanged?.Invoke(CurrentState as ClueGameState);
        }

        /**
         * Checks if the current state matches the specified state.
         * 
         * @param state The state to check against
         * @return True if the state matches, false otherwise
         */
        public bool IsInState(ClueGameState state)
        {
            return _currentState == state;
        }
    }
}
