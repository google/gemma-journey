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