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
 * ChatMessage.cs
 * 
 * Defines data structures for chat messages exchanged between
 * the user and AI models in the conversation system.
 */

namespace GoogleDeepMind.GemmaSampleGame
{
    /**
     * Represents the possible roles in a conversation:
     * User - Messages from the player
     * Model - Messages from the AI model (NPCs)
     */
    public enum ChatRole
{
    User,   // Player's messages
    Model   // AI/NPC responses
}

/**
 * Represents a single message in a conversation with its role and content.
 * Used to track conversation history and format messages for the AI model.
 */
public class ChatMessage
{
    /** The role of the entity that sent this message (user or model) */
    public ChatRole Role { get; }
    
    /** The text content of the message */
    public string Content { get; }

    /**
     * Creates a new chat message with the specified role and content.
     * 
     * @param role The role of the message sender (User or Model)
     * @param content The text content of the message
     */
    public ChatMessage(ChatRole role, string content)
    {
        Role = role;
        Content = content;
    }
}
}