
namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    /**
     * InputDoor.cs
     * 
     * Represents a door-related input event in the state machine system.
     * This input is generated when doors need to be opened, closed, or when
     * the player enters/exits through a door. States can respond to this input
     * to trigger appropriate transitions or behaviors.
     */
    using UnityEngine;

    public class InputDoor : StateInput
    {
        /**
         * Defines the types of door actions that can be performed.
         */
        public enum ActionType
        {
            /** Request to show a door */
            Show,
            /** Request to hide a door */
            Hide,
            /** Request to open a door */
            Open,
            /** Request to close a door */
            Close,
            /** Notification that player is entering through a door */
            Enter,
            /** Notification that player is exiting through a door */
            Exit
        }

        /** The specific type of door action being performed */
        public ActionType Type;

        /**
         * Creates a new door input event with the specified action type.
         * 
         * @param type The type of door action being performed
         */
        public InputDoor(ActionType type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"InputDoor: {Type}";
        }
    }

}