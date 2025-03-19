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
 * StateWalkAround.cs
 * 
 * Implements the main exploration state for the game state machine.
 * This state allows the player to move freely around the environment,
 * interact with NPCs, and enter doors to different rooms or levels.
 * 
 * The state handles transitions to dialogue when interacting with NPCs
 * and manages level changes when entering doors.
 */
using System;
using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    [Serializable]
    public class StateWalkAround : ClueGameState
    {
        /** Reference to the level manager for handling level transitions */
        [Inject] LevelManager _levelManager;

        /**
         * Called when entering the walk-around state.
         * Currently a placeholder for future functionality.
         */
        protected override void InternalEnterState()
        {
            // Currently empty, but could initialize exploration systems
        }

        /**
         * Called when exiting the walk-around state.
         * Currently a placeholder for future functionality.
         */
        protected override void InternalExitState()
        {
            // Currently empty, but could clean up exploration systems
        }

        /**
         * Updates the state each frame.
         * Checks for interaction inputs that would trigger state transitions.
         */
        public override void UpdateState()
        {
            if (_inputManager.HasInput<InputInteract>())
            {
                StateMachineChangeStateTo(_stateMachine.StateDialogue);
                return;
            }
            base.UpdateState();
        }

        /**
         * Internal update method for state-specific behavior.
         * Handles door interactions and level transitions.
         */
        protected override void InternalUpdateState()
        {
            if (_inputManager.HasInput<InputDoor>())
            {
                var input = _inputManager.GetLastInput<InputDoor>();
                if (input.Type == InputDoor.ActionType.Enter)
                {
                    LevelManager.LevelType nextLevelType = _levelManager.GetNextLevelType();
                    Debug.Log($"Next level: {nextLevelType}");
                    switch (nextLevelType)
                    {
                        case LevelManager.LevelType.Level:
                            _levelManager.ChangeLevel();
                            break;
                        case LevelManager.LevelType.Ending:
                            StateMachineChangeStateTo(_stateMachine.StateEndgame);
                            break;
                    }
                }
            }
        }

        /**
         * Checks if the transition into this state can be completed.
         * 
         * @return True, as this state has no special transition requirements
         */
        protected override bool TransitionIn()
        {
            return true;
        }

        /**
         * Checks if the transition out of this state can be completed.
         * 
         * @return True, as this state has no special transition requirements
         */
        protected override bool TransitionOut()
        {
            return true;
        }
    }
}