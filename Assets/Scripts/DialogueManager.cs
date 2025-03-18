/**
 * DialogueManager.cs
 * 
 * This singleton class manages the dialogue UI system in the game.
 * It handles displaying dialogue text, speaker names, and character portraits
 * using Unity's UI Toolkit (UIElements).
 * 
 * The manager uses a render texture and dedicated camera to create dynamic
 * character portraits during conversations, positioning the portrait camera
 * to capture the speaking character's face.
 */
using UnityEngine;
using UnityEngine.UIElements;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class DialogueManager : MonoBehaviour
{
    [SerializeField] private UIDocument dialogueDocument;
    [SerializeField] private Vector2 portraitSize = new Vector2(512, 512);
    [SerializeField] private Camera portraitCamera;

    private VisualElement root;
    private VisualElement dialogueContainer;
    private Label speakerNameLabel;
    private Label dialogueTextLabel;
    private Image portraitImage;
    private RenderTexture portraitRenderTexture;

    private static DialogueManager instance;
    public static DialogueManager Instance => instance;

    /// <summary>
    /// Initializes the dialogue manager as a singleton and sets up the UI elements and render texture.
    /// Ensures only one instance exists across scene changes.
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup UI
        root = dialogueDocument.rootVisualElement;
        dialogueContainer = root.Q<VisualElement>("dialogue-container");
        speakerNameLabel = root.Q<Label>("speaker-name");
        dialogueTextLabel = root.Q<Label>("dialogue-text");
        portraitImage = root.Q<Image>("portrait-image");

        // Setup render texture
        portraitRenderTexture = new RenderTexture((int)portraitSize.x, (int)portraitSize.y, 0);
        portraitRenderTexture.format = RenderTextureFormat.ARGB32;
        portraitCamera.targetTexture = portraitRenderTexture;
        portraitImage.image = portraitRenderTexture;

        HideDialogue();
    }

    /// <summary>
    /// Displays the dialogue UI with the specified text and speaker name.
    /// Positions the portrait camera to capture the speaking character's face.
    /// </summary>
    /// <param name="speakerName">Name of the character speaking</param>
    /// <param name="text">The dialogue text to display</param>
    /// <param name="npcTransform">Transform of the speaking character for portrait positioning</param>
    public void ShowDialogue(string speakerName, string text, Transform npcTransform)
    {
        dialogueContainer.style.display = DisplayStyle.Flex;
        speakerNameLabel.text = speakerName;
        dialogueTextLabel.text = text;

        // Position portrait camera
        portraitCamera.transform.position = npcTransform.position + new Vector3(0, 1.6f, 2f);
        portraitCamera.transform.LookAt(npcTransform.position + Vector3.up * 1.6f);
    }

    /// <summary>
    /// Hides the dialogue UI when a conversation ends or is interrupted.
    /// </summary>
    public void HideDialogue()
    {
        dialogueContainer.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Cleans up resources when the dialogue manager is destroyed.
    /// Releases the render texture to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        if (portraitRenderTexture != null)
        {
            portraitRenderTexture.Release();
        }
    }
}
}