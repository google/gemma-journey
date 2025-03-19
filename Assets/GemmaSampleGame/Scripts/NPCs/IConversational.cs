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
 * IConversational.cs
 * 
 * Interface that defines the contract for any entity that can engage in conversation
 * with the player. Implemented by NPCs and other interactive conversational objects.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    /**
     * Defines the required functionality for interactive conversational entities.
     * Provides methods for processing player messages, handling AI responses,
     * and managing conversation state.
     */
    public interface IConversational
{
    /** Gets the internal name/identifier of the conversational entity */
    string Name { get; }
    
    /** Gets the display name shown to the player during conversation */
    string DisplayName { get; }
    
    /** 
     * Processes a message from the player and generates a response.
     * 
     * @param message The player's message text
     */
    public void ListenPlayerMessage(string message);
    
    /**
     * Handles the complete response from the AI model.
     * 
     * @param response The full model response text
     */
    public void HandleFullResponse(string response);
    
    /**
     * Handles the end of a conversation when the player leaves.
     */
    public void LeaveConversation();
    
    /**
     * Resets the conversation state, clearing history.
     */
    public void ResetConversation();
    
    /**
     * Gets the formatted conversation history.
     * 
     * @return The formatted conversation history as a string
     */
    public string GetConversation();
}
}
