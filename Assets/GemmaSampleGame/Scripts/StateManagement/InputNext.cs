/**
 * InputNext.cs
 * 
 * Represents a request to advance to the next step in a sequence.
 * This input is generated when the player indicates they want to proceed
 * to the next part of a dialogue, tutorial, or other sequential content.
 * The state machine responds by advancing the current state accordingly.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputNext : StateInput
    {
        // This class is used as a signal input with no additional data
    }
}