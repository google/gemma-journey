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
 * InputInteract.cs
 * 
 * Represents an interaction input event in the state machine system.
 * This input is generated when the player initiates interaction with a conversational
 * agent such as an NPC. States can respond to this input to transition to dialogue
 * states or trigger other interaction behaviors.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputInteract : StateInput
    {
        /** The conversational agent (typically an NPC) that the player is interacting with */
        public IConversational InteractingAgent { get; private set; }

        /**
         * Creates a new interaction input event with the specified conversational agent.
         * 
         * @param interactingAgent The conversational agent being interacted with
         */
        public InputInteract(IConversational interactingAgent)
        {
            InteractingAgent = interactingAgent;
        }
    }

}