/**
 * InputPlayerMessage.cs
 * 
 * Represents a message input event from the player in the state machine system.
 * This input is generated when the player sends a text message during dialogue
 * with an NPC. The message text is stored for processing by the appropriate
 * dialogue handling states.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputPlayerMessage : StateInput
    {
        /** The text content of the player's message */
        private string message;

        /** Public accessor for the message content */
        public string Message => message;

        /**
         * Creates a new player message input with the specified text content.
         * 
         * @param message The text content of the player's message
         */
        public InputPlayerMessage(string message)
        {
            this.message = message;
        }
    }
}