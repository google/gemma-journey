/**
 * InputSound.cs
 * 
 * Represents a sound toggle input event in the state machine system.
 * This input is generated when the player toggles sound on or off
 * through UI or keyboard shortcuts. The state machine uses this input
 * to update the game's audio settings.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputSound : StateInput
    {
        /** Whether sound should be enabled or disabled */
        private bool _enabled;

        /** Public accessor for the sound enabled state */
        public bool Enabled => _enabled;

        /**
         * Creates a new sound toggle input with the specified enabled state.
         * 
         * @param enabled True to enable sound, false to disable sound
         */
        public InputSound(bool enabled)
        {
            _enabled = enabled;
        }
    }

}