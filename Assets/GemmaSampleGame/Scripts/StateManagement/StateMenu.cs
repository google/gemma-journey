/**
 * StateMenu.cs
 * 
 * Implements the main menu state for the game state machine.
 * This state displays the main menu UI, handles player input for menu options,
 * and manages transitions to other states when the player selects options
 * like "Play" or "Tutorial".
 * 
 * The state coordinates multiple UI elements including the main menu,
 * control overlay, and background elements.
 */
using System;
using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    [Serializable]
    public class StateMenu : ClueGameState
    {
        /** Reference to the main menu UI component */
        [Inject] private UI.MainMenuUserInterface mainMenuUserInterface;

        /** Reference to the control overlay UI that shows button hints */
        [Inject] private UI.ControlOverlayUserInterface controlOverlayUserInterface;

        /** Reference to the backdrop UI that provides background visuals */
        [Inject] private UI.BackdropUserInterface backdropUserInterface;

        /**
         * Called when entering the menu state.
         * Opens all necessary UI elements and configures the control overlay.
         */
        protected override void InternalEnterState()
        {
            backdropUserInterface.Open();
            mainMenuUserInterface.Open();

            controlOverlayUserInterface.HideAll();
            controlOverlayUserInterface.ToggleText(true);
            controlOverlayUserInterface.Open();
        }

        /**
         * Called when exiting the menu state.
         * Closes UI elements that are no longer needed.
         */
        protected override void InternalExitState()
        {
            mainMenuUserInterface.Close();
            backdropUserInterface.Close();
        }

        /**
         * Updates the state each frame.
         * Checks for play input to start the game.
         */
        public override void UpdateState()
        {
            if (_inputManager.HasInput<InputPlay>())
            {
                StateMachineChangeStateTo(_stateMachine.StateTutorial);
                return;
            }
            base.UpdateState();
        }

        /**
         * Internal update method for state-specific behavior.
         * Currently empty but could be used for menu animations or effects.
         */
        protected override void InternalUpdateState()
        {
            // Currently empty, could be used for menu-specific updates
        }

        /**
         * Checks if the transition into this state can be completed.
         * Waits for UI elements to be ready before completing the transition.
         * 
         * @return True if all UI elements are ready, false otherwise
         */
        protected override bool TransitionIn()
        {
            if (mainMenuUserInterface.IsReady && controlOverlayUserInterface.IsReady)
            {
                return true;
            }
            return false;
        }

        /**
         * Called when the transition into this state is complete.
         * Performs finalization like closing the backdrop.
         */
        public override void TransitionInDone()
        {
            base.TransitionInDone();
            backdropUserInterface.Close();
        }

        /**
         * Checks if the transition out of this state can be completed.
         * Waits for UI elements to be ready before completing the transition.
         * 
         * @return True if the main menu UI is ready, false otherwise
         */
        protected override bool TransitionOut()
        {
            if (mainMenuUserInterface.IsReady)
            {
                return true;
            }
            return false;
        }
    }
}