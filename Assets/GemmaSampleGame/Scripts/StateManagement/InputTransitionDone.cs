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
 * InputTransitionDone.cs
 * 
 * Represents the completion of a UI transition in the state machine system.
 * This input is generated when a UI transition animation (fade, slide, etc.)
 * has completed. The state machine uses this to coordinate state changes
 * with UI animations, ensuring smooth visual transitions between states.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputTransitionDone : StateInput
    {
        /** Reference to the UI element that has completed its transition */
        private UI.UserInterface _userInterface;

        /** Public accessor for the UI element */
        public UI.UserInterface UserInterface => _userInterface;

        /**
         * Creates a new transition completion input with the specified UI element.
         * 
         * @param userInterface The UI element that has completed its transition
         */
        public InputTransitionDone(UI.UserInterface userInterface)
        {
            _userInterface = userInterface;
        }
    }

}