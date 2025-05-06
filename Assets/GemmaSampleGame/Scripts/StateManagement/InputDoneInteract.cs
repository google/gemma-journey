
namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    /**
     * InputDoneInteract.cs
     * 
     * Represents a completion input event for NPC interactions in the state machine system.
     * This input is generated when an interaction with an NPC is complete and the player
     * should return to the exploration state. The state machine responds to this input
     * by transitioning from dialogue states back to exploration states after a delay.
     */
    using UnityEngine;

    public class InputDoneInteract : StateInput
    {
        // This class is used as a signal input with no additional data
    }
}