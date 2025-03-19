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

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class PersonalityProvider : MonoBehaviour
{
    [SerializeField] private NPCLocalSettings localSetting;
    [SerializeField] private string localSettingFilename;

    private NPCGlobalSettings npcGlobalSettings;
    private const string folderName = "Personality";
    private const string fileType = ".txt";

    [Inject]
    public void Construct(NPCGlobalSettings npcGlobalSettings)
    {
        this.npcGlobalSettings = npcGlobalSettings;
    }
    public static event Action<string> OnPersonalityStatus;

    private string characterName = string.Empty;
    private string fileSystemPrompt = string.Empty;

    public string Name => GetName();
    public string SystemPrompt => GetSystemPrompt();

    private void Start()
    {
        ReloadPersonality();
    }
    protected virtual string GetName()
    {
        if (!string.IsNullOrEmpty(characterName))
        {
            return characterName;
        }
        return localSetting.Name;
    }

    protected virtual string GetSystemPrompt()
    {
        if (!string.IsNullOrEmpty(fileSystemPrompt))
        {
            return fileSystemPrompt;
        }
        return $"{npcGlobalSettings.GlobalPromptPrefix}\n\n{localSetting.Personality}\n\n{npcGlobalSettings.GlobalPromptSuffix}\n\n{npcGlobalSettings.LengthConstraint}";
    }

    public virtual void ReloadPersonality()
    {
        characterName = string.Empty;
        fileSystemPrompt = string.Empty;
        string folderPath = Path.Combine(Application.dataPath, folderName);
        if (!Directory.Exists(folderPath))
        {
            OnPersonalityStatus?.Invoke($"Folder {folderPath} does not existed. Please create and put personality files under this directory\nUsing default personality");
            return;
        }

        string localSettingPath = Path.Combine(folderPath, localSettingFilename + fileType);
        if (!File.Exists(localSettingPath))
        {
            OnPersonalityStatus?.Invoke($"File {localSettingFilename} does not existed. Please create {localSettingFilename} in {folderPath}.\nUsing default personality");
            return;
        }

        try
        {
            string[] allLine = File.ReadAllLines(localSettingPath);
            if (allLine.Length == 0)
            {
                OnPersonalityStatus?.Invoke($"Text from {localSettingPath} is not available. Using default personality");
                return;
            }
            characterName = allLine[0];
            fileSystemPrompt = File.ReadAllText(localSettingPath).Substring(characterName.Length);
            OnPersonalityStatus?.Invoke($"{localSettingFilename} loaded. Current character name is: {characterName}");
        } 
        catch (Exception e)
        {
            OnPersonalityStatus?.Invoke(e.ToString());
            Debug.LogException(e);
        }
    }
}
}
