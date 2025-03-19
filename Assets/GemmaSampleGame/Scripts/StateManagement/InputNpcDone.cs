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
 * InputNpcDone.cs
 * 
 * Represents a signal that an NPC interaction has been successfully completed.
 * This input is generated when the player has successfully completed a conversation
 * challenge with an NPC. The state machine uses this input to trigger door opening
 * or other progression mechanisms in the game.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputNpcDone : StateInput
    {
        // This class is used as a signal input with no additional data
    }
}