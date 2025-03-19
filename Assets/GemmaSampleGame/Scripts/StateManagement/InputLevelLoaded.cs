// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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