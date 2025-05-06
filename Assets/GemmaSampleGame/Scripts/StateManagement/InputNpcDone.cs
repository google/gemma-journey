/**
 * InputNpcDone.cs
 * 
 * Represents a signal that an NPC interaction has been successfully completed.
 * This input is generated when the player has successfully completed a conversation
 * challenge with an NPC. The state machine uses this input to trigger door opening
 * or other progression mechanisms in the game.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputNpcDone : StateInput
    {
        // This class is used as a signal input with no additional data
    }
}