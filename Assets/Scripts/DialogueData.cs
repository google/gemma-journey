/**
 * DialogueData.cs
 * 
 * This class manages dialogue data for NPCs or interactive objects in the game.
 * It stores a sequence of dialogue lines with optional voice clips and handles
 * the progression through these lines when the player interacts with the dialogue.
 * 
 * The class works in conjunction with the DialogueManager to display text and
 * play voice lines at appropriate times.
 */
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    /// <summary>
    /// Represents a single line of dialogue with optional voice audio.
    /// </summary>
    [System.Serializable]
    public class DialogueLine
{
    [TextArea(3, 10)]
    public string text;
    public AudioClip voiceLine;  // Optional
}

public class DialogueData : MonoBehaviour
{
    [SerializeField] private string speakerName;
    [SerializeField] private DialogueLine[] dialogueLines;
    private int currentLine = 0;

    /// <summary>
    /// Initiates a dialogue sequence by resetting to the first line and displaying it.
    /// Called when the player first interacts with this dialogue source.
    /// </summary>
    public void StartDialogue()
    {
        currentLine = 0;
        ShowCurrentLine();
    }

    /// <summary>
    /// Displays the current dialogue line and plays its associated voice clip if available.
    /// If all lines have been shown, hides the dialogue UI.
    /// </summary>
    public void ShowCurrentLine()
    {
        if (currentLine < dialogueLines.Length)
        {
            DialogueManager.Instance.ShowDialogue(
                speakerName,
                dialogueLines[currentLine].text,
                transform
            );

            // Play voice line if available
            if (dialogueLines[currentLine].voiceLine != null)
            {
                AudioSource.PlayClipAtPoint(
                    dialogueLines[currentLine].voiceLine,
                    Camera.main.transform.position
                );
            }
        }
        else
        {
            DialogueManager.Instance.HideDialogue();
        }
    }

    /// <summary>
    /// Advances to the next line of dialogue and displays it.
    /// Called when the player requests to continue the conversation.
    /// </summary>
    public void AdvanceDialogue()
    {
        currentLine++;
        ShowCurrentLine();
    }
}
}