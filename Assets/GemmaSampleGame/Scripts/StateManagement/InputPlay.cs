/**
 * InputPlay.cs
 * 
 * Represents a request to start gameplay from a menu or title screen.
 * This input is generated when the player selects "Play" or a similar option
 * from the main menu. The state machine responds by transitioning from
 * menu states to gameplay states, loading the appropriate level.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputPlay : StateInput
    {
        // This class is used as a signal input with no additional data
    }
}