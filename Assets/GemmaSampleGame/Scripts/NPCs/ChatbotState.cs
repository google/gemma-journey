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
 * ChatbotState.cs
 * 
 * Defines the possible states of an NPC during conversation.
 * Used to control animations, UI, and behavior based on the NPC's
 * current conversational state.
 */

namespace GoogleDeepMind.GemmaSampleGame
{
    /**
     * Represents the possible states of an NPC during conversation.
     */
    public enum ChatbotState
{
    /** Initial state before first interaction */
    Initial,
    
    /** NPC is not engaged in conversation, in neutral state */
    Idle,
    
    /** NPC is processing a player message and generating a response */
    Thinking,
    
    /** NPC is delivering a response to the player */
    Speaking,
    
    /** NPC is waiting for player input */
    Listening,
}
}