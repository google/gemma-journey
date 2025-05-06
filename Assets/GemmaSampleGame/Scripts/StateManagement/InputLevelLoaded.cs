/**
 * InputLevelLoaded.cs
 * 
 * Represents a notification that a level has been loaded in the state machine system.
 * This input is generated when a scene finishes loading asynchronously, and contains
 * information about the type of level that was loaded (regular level, menu, ending, etc.).
 * The state machine uses this input to transition to the appropriate state based on the
 * loaded level type.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputLevelLoaded : StateInput
    {
        /** The type of level that was loaded */
        public LevelManager.LevelType LevelType;

        /**
         * Creates a new level loaded input with the specified level type.
         * 
         * @param levelType The type of level that was loaded
         */
        public InputLevelLoaded(LevelManager.LevelType levelType)
        {
            LevelType = levelType;
        }
    }
}