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