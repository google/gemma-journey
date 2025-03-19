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
     * InputDoneInteract.cs
     * 
     * Represents a completion input event for NPC interactions in the state machine system.
     * This input is generated when an interaction with an NPC is complete and the player
     * should return to the exploration state. The state machine responds to this input
     * by transitioning from dialogue states back to exploration states after a delay.
     */
    using UnityEngine;

    public class InputDoneInteract : StateInput
    {
        // This class is used as a signal input with no additional data
    }
}