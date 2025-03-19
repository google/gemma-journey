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
 * InputPlayerMessage.cs
 * 
 * Represents a message input event from the player in the state machine system.
 * This input is generated when the player sends a text message during dialogue
 * with an NPC. The message text is stored for processing by the appropriate
 * dialogue handling states.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputPlayerMessage : StateInput
    {
        /** The text content of the player's message */
        private string message;

        /** Public accessor for the message content */
        public string Message => message;

        /**
         * Creates a new player message input with the specified text content.
         * 
         * @param message The text content of the player's message
         */
        public InputPlayerMessage(string message)
        {
            this.message = message;
        }
    }
}