/**
 * StateStart.cs
 * 
 * Implements the initial state for the game state machine.
 * This state serves as the entry point when the game first launches
 * and automatically transitions to the main menu after a brief delay
 * or when the player provides input.
 * 
 * The state uses a timer to automatically advance if the player
 * doesn't provide input within a certain timeframe.
 */
using System;
using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    [Serializable]
    public class StateStart : ClueGameState
    {
        /** Timestamp when the state was entered */
        private float startTime = 0;

        /** Time to wait before automatically advancing (currently set to immediate) */
        private float waitTime = 0;

        /**
         * Called when entering the start state.
         * Records the current time to calculate when to auto-advance.
         */
        protected override void InternalEnterState()
        {
            startTime = Time.time;
        }

        /**
         * Called when exiting the start state.
         * Currently a placeholder for future functionality.
         */
        protected override void InternalExitState()
        {
            // Currently empty, but could clean up resources if needed
        }

        /**
         * Updates the state each frame.
         * Checks for next input to advance to the menu.
         */
        public override void UpdateState()
        {
            if (_inputManager.HasInput<InputNext>())
            {
                StateMachineChangeStateTo(_stateMachine.StateLoading);
            }
            base.UpdateState();
        }

        /**
         * Internal update method for state-specific behavior.
         * Automatically generates a next input after the wait time has elapsed.
         */
        protected override void InternalUpdateState()
        {
            if (Time.time - startTime > waitTime)
            {
                _inputManager.AddInput(new InputNext());
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