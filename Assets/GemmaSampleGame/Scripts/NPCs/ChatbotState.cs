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