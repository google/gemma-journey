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
 * SecretManager.cs
 * 
 * Manages secret words associated with NPCChatbot characters in the game.
 * 
 * This class provides functionality to track, store, and retrieve secret words
 * for each character, supporting the core gameplay mechanic of secret discovery.
 * The secret words are stored in a dictionary with the bot's GameObject name as the key.
 */
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class SecretManager
{
    /** Dictionary mapping NPC names to their secret words */
    private Dictionary<string, string> secrets = new();

    /**
     * Initializes the secret dictionary with empty strings for each bot.
     * 
     * @param bots List of NPCChatbot instances to register in the secret manager
     */
    public void SetupSecretMap(List<NPCChatbot> bots)
    {
        foreach (NPCChatbot bot in bots)
        {
            secrets.Add(bot.gameObject.name, "");
        }
    }
    
    /**
     * Adds or updates a secret word for a specific bot.
     * 
     * @param bot The NPCChatbot whose secret word is being set
     * @param secret The secret word to associate with the bot
     */
    public void AddSecretWord(NPCChatbot bot, string secret)
    {
        if (!secrets.ContainsKey(bot.gameObject.name))
        {
            secrets.Add(bot.gameObject.name, secret);
        }
        else
        {
            secrets[bot.gameObject.name] = secret;
        }
    }

    /**
     * Retrieves the secret word associated with a specific bot.
     * If no secret exists, creates an empty entry and returns an empty string.
     * 
     * @param bot The NPCChatbot whose secret word is being requested
     * @return The secret word as a string
     */
    public string GetSecretWord(NPCChatbot bot)
    {
        if (secrets.ContainsKey(bot.gameObject.name))
        {
            return secrets[bot.gameObject.name];
        }
        else
        {
            secrets.Add(bot.gameObject.name, string.Empty);
            return string.Empty;
        }
    }

    /**
     * Serializes the secrets dictionary to a JSON string.
     * 
     * @return JSON string representation of all secrets
     */
    public override string ToString()
    {
        return JsonConvert.SerializeObject(secrets);
    }

    /**
     * Retrieves a list of all secret words currently stored.
     * 
     * @return List of all secret words
     */
    public List<string> GetSecretWords()
    {
        List<string> words = new List<string>();
        foreach (string item in secrets.Values)
        {
            words.Add(item);
        }
        return words;
    }
}
}
