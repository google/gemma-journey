/**
 * InputTransitionStart.cs
 * 
 * Represents the start of a UI transition in the state machine system.
 * This input is generated when a UI transition animation (fade, slide, etc.)
 * is about to begin. The state machine uses this to coordinate state changes
 * with UI animations, ensuring states don't change before transitions have started.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputTransitionStart : StateInput
    {
        // This class is used as a signal input with no additional data
    }

}