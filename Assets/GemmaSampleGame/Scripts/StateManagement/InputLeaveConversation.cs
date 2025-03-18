/**
 * InputLeaveConversation.cs
 * 
 * Represents a user request to exit a conversation in the state machine system.
 * This input is generated when the player explicitly chooses to end a conversation
 * with an NPC, typically by clicking a "Leave" button. The state machine and NPC
 * respond to this input by playing exit animations and transitioning to the
 * appropriate state.
 */
using UnityEngine;


namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputLeaveConversation : StateInput
    {
        // This class is used as a signal input with no additional data
    }

}