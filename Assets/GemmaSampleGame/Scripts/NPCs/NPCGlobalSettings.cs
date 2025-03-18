/**
 * NPCGlobalSettings.cs
 * 
 * Scriptable object that defines global settings for all NPCs in the game.
 * Includes prompt templates for the AI model, conversation constraints,
 * and standard response formats that apply to all NPCs.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    [CreateAssetMenu(fileName = "NPCGlobalSettings", menuName = "Gemma Sample Game/NPC Global Settings")]
    public class NPCGlobalSettings : ScriptableObject
{
    [Header("Prompts and Prompt Fragments")]

    /** Prompt fragment used when the player is leaving a conversation */
    [SerializeField, TextArea(3, 10)]
    private string globalLeavingConversationPrompt;

    /** Standard prefix for all NPC system prompts that establishes the roleplay context */
    [SerializeField, TextArea(3, 10)]
    private string globalPromptPrefix = @"You are a character in a role-playing game. Review the following character description and reply to all subsequent prompts as though you are this character. Say OK when you have completed review of the character description.";

    /** Standard suffix for all NPC system prompts with general behavior guidelines */
    [SerializeField, TextArea(3, 10)]
    private string globalPromptSuffix = @"
Keep your responses brief and natural. Stay in character at all times.
If asked about things your character wouldn't know about, respond with uncertainty or confusion.";

    /** Maximum number of paragraphs allowed in NPC responses */
    [SerializeField]
    private int maxParagraphs = 3;

    /** Maximum characters per paragraph in NPC responses */
    [SerializeField, Range(50, 300)]
    private int maxCharsPerParagraph = 200;

    /** Public accessor for the leaving conversation prompt */
    public string GlobalLeavingConversationPrompt => globalLeavingConversationPrompt;

    /** Public accessor for the global prompt prefix */
    public string GlobalPromptPrefix => globalPromptPrefix;

    /** Public accessor for the global prompt suffix */
    public string GlobalPromptSuffix => globalPromptSuffix;

    /**
     * Generates a constraint string to limit the length of NPC responses.
     * Used as part of the system prompt to keep responses concise.
     * 
     * @return Formatted constraint string with paragraph and character limits
     */
    public string LengthConstraint =>
        $"Answer with at most {maxParagraphs} paragraphs wherein each paragraph is at most {maxCharsPerParagraph} characters.";
}
}