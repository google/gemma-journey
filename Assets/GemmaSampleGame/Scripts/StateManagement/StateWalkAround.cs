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
            else if (_inputManager.HasInput<InputLevelLoaded>())
            {
                _inputManager.AddInput(new InputDoor(InputDoor.ActionType.Show));
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