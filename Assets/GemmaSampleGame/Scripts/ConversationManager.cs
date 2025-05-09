/**
 * ConversationManager.cs
 * 
 * Central manager for handling conversations between the player and NPCs.
 * Acts as an intermediary that routes player messages to the currently active
 * conversational agent (NPC) and provides methods to control the conversation flow.
 * 
 * This class maintains a reference to the currently active chat and ensures
 * that messages are properly directed to the appropriate NPC.
 */
using UnityEngine;
using VContainer;
using GemmaCpp;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class ConversationManager
{
    /** Reference to the currently active conversational agent (typically an NPC) */
    private IConversational activeChat;

    /** Public accessor for the currently active conversational agent */
    public IConversational ActiveChat => activeChat;

    private GemmaManager gemmaManager;
    [Inject]
    public void Construct(GemmaManager gemmaManager)
    {
        this.gemmaManager = gemmaManager;
    }

    /**
     * Sets the active conversational agent for the current dialogue session.
     * If the new agent is different from the current one, updates the reference.
     * 
     * @param chatbot The conversational agent to set as active
     */
    public void SetActiveChat(IConversational chatbot)
    {
        if (activeChat != chatbot)
        {
            Debug.Log($"Setting active chat to {(chatbot != null ? chatbot.DisplayName : "null")}");
            activeChat = chatbot;
        }
        var conversationName = chatbot?.Name ?? "default";
        Debug.Log($"Switching active conversation in GemmaInterop to: {conversationName}");
        if (!gemmaManager.HasConversation(conversationName)) {
            gemmaManager.CreateConversation(conversationName);            
        }
        gemmaManager.SwitchConversation(conversationName);
    }

    /**
     * Sends a message from the player to the currently active conversational agent.
     * Logs a warning if there is no active agent to receive the message.
     * 
     * @param message The text message from the player
     */
    public void SendMessage(string message)
    {
        if (activeChat != null)
        {
            activeChat.ListenPlayerMessage(message);
        }
        else
        {
            Debug.LogWarning("Tried to send message but no active chat is set!");
        }
    }

    /**
     * Initiates the process of leaving the current conversation.
     * Notifies the active conversational agent that the player is leaving.
     * Logs a warning if there is no active conversation to leave.
     */
    public void LeaveConversation()
    {
        if (activeChat != null)
        {
            Debug.Log("[ConversationManager] Leaving conversation");
            activeChat.LeaveConversation();
        }
        else
        {
            Debug.LogWarning("[ConversationManager] Tried to leave conversation but no active chat is set!");
        }
    }

    /**
     * Resets the current conversation to its initial state.
     * Clears conversation history and resets the agent's state.
     */
    public void ResetConversation()
    {
        if (activeChat != null)
        {
            activeChat.ResetConversation();
        }
    }

    /**
     * Retrieves the full conversation history from the active agent.
     * 
     * @return The formatted conversation history, or null if no active agent
     */
    public string GetConversation()
    {
        if (activeChat != null)
        {
            return activeChat.GetConversation();
        }
        return null;
    }
}
}