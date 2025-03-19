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
 * NPCLocalSettings.cs
 * 
 * Scriptable object that defines the personality and behavior settings for an NPC.
 * 
 * This asset contains the prompts and roles that define an NPC's character, 
 * and provides methods to generate system prompts for AI-based dialogue generation.
 * Each NPC can have multiple prompts and roles that combine to create its unique personality.
 */
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame
{
    [CreateAssetMenu(fileName = "NPCLocalSettings", menuName = "Gemma Sample Game/NPC Local Settings")]
    public class NPCLocalSettings : ScriptableObject
{
    /** Format string for prompt section headers */
    private const string promptNameFormat = "### {0}\n\n";

    /**
     * Structure that defines a prompt section with a name and content.
     * Each section contributes to the NPC's overall personality.
     */
    [Serializable]
    internal struct Prompt
    {
        /** Name of this prompt section */
        [SerializeField]
        internal string promptName;

        /** Content of this prompt section */
        [SerializeField, TextArea(3, 10)]
        internal string prompt;
    }

    /** The display name of the NPC */
    [SerializeField] private string npcName = "Villager";

    /** Array of prompt sections that define the NPC's personality */
    [SerializeField] private Prompt[] prompts;

    /** Array of general settings that define the NPC's roles */
    [SerializeField] private NPCGeneralSetting[] roles;

    /** Gets the display name of the NPC */
    public string Name => npcName;

    /** Gets the complete personality prompt for the NPC */
    public string Personality => GetSystemPrompt();

    /**
     * Generates the complete system prompt by combining all prompt sections and roles.
     * Each prompt is formatted with its name followed by its content.
     * Prompts are separated by dividers, and roles are appended at the end.
     * 
     * @return The complete system prompt as a string
     */
    public virtual string GetSystemPrompt()
    {
        Debug.Log($"GetSystemPrompt called, prompt count: {prompts.Length}, roles count: {roles.Length}");
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
        if (roles.Length > 0)
        {
            promptBuilder.Append("\n\n---\n\n");
        }
        for (int i = 0; i < roles.Length; i++)
        {
            promptBuilder.Append(roles[i].GetFormattedPrompt());
        }
        return promptBuilder.ToString();
    }
}
}
