/**
 * InputQuit.cs
 * 
 * Represents a request to exit the game in the state machine system.
 * This input is generated when the player selects a quit or exit option,
 * typically from a menu. The state machine responds by initiating the
 * application shutdown process.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputQuit : StateInput
    {
        // This class is used as a signal input with no additional data
    }
}
