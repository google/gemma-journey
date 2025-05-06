using System;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    [Serializable]
    public class StateLoading : ClueGameState
    {
        /** Reference to the loading UI component */
        [Inject] private UI.LoadingUserInterface loadingUserInterface;
        /** Reference to the backdrop UI that provides background visuals */
        [Inject] private UI.BackdropUserInterface backdropUserInterface;

        /**
         * Called when entering the menu state.
         * Opens all necessary UI elements and configures the control overlay.
         */
        protected override void InternalEnterState()
        {
            backdropUserInterface.Open();
            loadingUserInterface.Open();
        }

         /**
          * Called when exiting the menu state.
          * Closes UI elements that are no longer needed.
          */
        protected override void InternalExitState()
        {
            loadingUserInterface.Close();
        }

        public override void UpdateState()
        {
            if (_inputManager.HasInput<InputLoaded>())
            {
                StateMachineChangeStateTo(_stateMachine.StateMenu);
                return;
            }
            base.UpdateState();
        }

        protected override void InternalUpdateState()
        {
            // Currently empty, could be used for loading-specific updates
        }

        /**
         * Checks if the transition into this state can be completed.
         * Waits for UI elements to be ready before completing the transition.
         * 
         * @return True if all UI elements are ready, false otherwise
         */
        protected override bool TransitionIn()
        {
            if (loadingUserInterface.IsReady)
            {
                return true;
            }
            return false;
        }

        /**
         * Checks if the transition out of this state can be completed.
         * Waits for UI elements to be ready before completing the transition.
         * 
         * @return True if the main menu UI is ready, false otherwise
         */
        protected override bool TransitionOut()
        {
            if (loadingUserInterface.IsReady)
            {
                return true;
            }
            return false;
        }
    }
}