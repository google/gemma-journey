using System.Text;
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    [CreateAssetMenu(fileName = "NPCGeneralSetting", menuName = "Gemma Sample Game/NPC General Setting")]
    public class NPCGeneralSetting : ScriptableObject
{
    private const string promptNameFormat = "### {0}\n\n";
    [SerializeField] private NPCLocalSettings.Prompt[] prompts;

    public string GetFormattedPrompt()
    {
        StringBuilder promptBuilder = new StringBuilder();
        for (int i = 0; i < prompts.Length; i++)
        {
            promptBuilder.AppendFormat(promptNameFormat, prompts[i].promptName);
            promptBuilder.Append(prompts[i].prompt);
            if (i < prompts.Length - 1)
            {
                promptBuilder.Append("\n\n---\n\n");
            }
        }
        return promptBuilder.ToString();
    }
}
}
