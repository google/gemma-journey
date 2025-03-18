
/**
 * InputInteract.cs
 * 
 * Represents an interaction input event in the state machine system.
 * This input is generated when the player initiates interaction with a conversational
 * agent such as an NPC. States can respond to this input to transition to dialogue
 * states or trigger other interaction behaviors.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputInteract : StateInput
    {
        /** The conversational agent (typically an NPC) that the player is interacting with */
        public IConversational InteractingAgent { get; private set; }

        /**
         * Creates a new interaction input event with the specified conversational agent.
         * 
         * @param interactingAgent The conversational agent being interacted with
         */
        public InputInteract(IConversational interactingAgent)
        {
            InteractingAgent = interactingAgent;
        }
    }

}