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


namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    /**
     * InputDoor.cs
     * 
     * Represents a door-related input event in the state machine system.
     * This input is generated when doors need to be opened, closed, or when
     * the player enters/exits through a door. States can respond to this input
     * to trigger appropriate transitions or behaviors.
     */
    using UnityEngine;

    public class InputDoor : StateInput
    {
        /**
         * Defines the types of door actions that can be performed.
         */
        public enum ActionType
        {
            /** Request to open a door */
            Open,
            /** Request to close a door */
            Close,
            /** Notification that player is entering through a door */
            Enter,
            /** Notification that player is exiting through a door */
            Exit
        }

        /** The specific type of door action being performed */
        public ActionType Type;

        /**
         * Creates a new door input event with the specified action type.
         * 
         * @param type The type of door action being performed
         */
        public InputDoor(ActionType type)
        {
            Type = type;
        }
    }

}