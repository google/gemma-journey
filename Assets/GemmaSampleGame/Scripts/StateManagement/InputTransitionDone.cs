/**
 * InputTransitionDone.cs
 * 
 * Represents the completion of a UI transition in the state machine system.
 * This input is generated when a UI transition animation (fade, slide, etc.)
 * has completed. The state machine uses this to coordinate state changes
 * with UI animations, ensuring smooth visual transitions between states.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputTransitionDone : StateInput
    {
        /** Reference to the UI element that has completed its transition */
        private UI.UserInterface _userInterface;

        /** Public accessor for the UI element */
        public UI.UserInterface UserInterface => _userInterface;

        /**
         * Creates a new transition completion input with the specified UI element.
         * 
         * @param userInterface The UI element that has completed its transition
         */
        public InputTransitionDone(UI.UserInterface userInterface)
        {
            _userInterface = userInterface;
        }
    }

}