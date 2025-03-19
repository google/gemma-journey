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
 * InputTransitionStart.cs
 * 
 * Represents the start of a UI transition in the state machine system.
 * This input is generated when a UI transition animation (fade, slide, etc.)
 * is about to begin. The state machine uses this to coordinate state changes
 * with UI animations, ensuring states don't change before transitions have started.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public class InputTransitionStart : StateInput
    {
        // This class is used as a signal input with no additional data
    }

}