
using Cysharp.Threading.Tasks;
using GemmaCpp;
using GoogleDeepMind.GemmaSampleGame.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using static GemmaCpp.GemmaManager;

namespace GoogleDeepMind.GemmaSampleGame
{

    public class NPCPrewarmer
    {
        private NPCGlobalSettings npcGlobalSettings;
        private GemmaManager gemmaManager;
        private StateInputManager stateInputManager;

        private Dictionary<string, PrewarmState> initialized = new Dictionary<string, PrewarmState>();

        public PrewarmState CheckPrewarmState(string npcName)
        {
            var state = initialized.ContainsKey(npcName)
                ? initialized[npcName] : PrewarmState.NotApplicable;
            return state;
        }

        [Inject]
        public void Construct(GemmaManager gemmaManager, NPCGlobalSettings npcGlobalSettings, StateInputManager stateInputManager)
        {
            this.gemmaManager = gemmaManager;
            this.npcGlobalSettings = npcGlobalSettings;
            this.stateInputManager = stateInputManager;  

            foreach (var npc in npcGlobalSettings.NPCLocalSettingsToPrewarm)
            {
                initialized.Add(npc.ConversationName, PrewarmState.Pending);
            }
        }

        // Prewarm the next NPC.
        public void PrewarmOne()
        {
            var targetKvp = initialized.Where(kvp => kvp.Value.Equals(PrewarmState.Pending)).FirstOrDefault();

            if (string.IsNullOrEmpty(targetKvp.Key))
            {
                Debug.Log($"PrewarmOne(): [{Time.time:F3}] No pending NPCs found to prewarm.");
                return;
            }

            var target = targetKvp.Key;
            var npc = npcGlobalSettings.NPCLocalSettingsToPrewarm.FirstOrDefault(s => s.ConversationName.Equals(target));

            if (npc == null)
            {
                Debug.LogError($"PrewarmOne(): [{Time.time:F3}] Could not find NPC settings for pending target '{target}'. Skipping.");
                // Consider marking as error: initialized[target] = PrewarmState.Error;
                return;
            }

            StringBuilder warmUpResponseBuilder = new StringBuilder();
            warmUpResponseBuilder.AppendLine(npcGlobalSettings.GlobalPromptPrefix);
            warmUpResponseBuilder.AppendLine();
            warmUpResponseBuilder.AppendLine(npc.Personality);
            warmUpResponseBuilder.AppendLine();
            warmUpResponseBuilder.AppendLine(npcGlobalSettings.GlobalPromptSuffix);

            // prewarm one item
            gemmaManager.Prewarm(new Dictionary<string, string>() {
                {
                    target,
                    warmUpResponseBuilder.ToString()
                }
            }, (conversation, state) => {
                var oldState = initialized[conversation];
                initialized[conversation] = state;
                Debug.Log($"PrewarmOne(): {Time.time:F3} {conversation} {oldState.ToString()} -> {state.ToString()} ");
            }).Forget();
        }

        // Prewarm all NPCs. Takes about 20s per NPC on Gemma3 4B on a Ryzen 9 6800, so really just here as a reference.
        public async UniTask Prewarm()
        {
            Dictionary<string, string> prewarmData = new Dictionary<string, string>();
            // Using Time.time for elapsed game time, formatted to 3 decimal places (F3)
            string timestamp = $"[{Time.time:F3}]";
            Debug.Log($"Prewarm(): {timestamp} Prewarm sequence started. Waiting for GemmaManager initialization...");

            timestamp = $"[{Time.time:F3}]";
            Debug.Log($"Prewarm(): {timestamp} GemmaManager initialized. Starting NPC conversation prewarming.");

            StringBuilder warmUpResponseBuilder = new StringBuilder();


            int count = 0;
            foreach (NPCLocalSettings npc in npcGlobalSettings.NPCLocalSettingsToPrewarm)
            {
                warmUpResponseBuilder.Clear();
                timestamp = $"[{Time.time:F3}]";
                if (npc == null || string.IsNullOrEmpty(npc.ConversationName))
                {
                    Debug.LogWarning($"Prewarm(): {timestamp} Skipping NPC at index {count} due to missing settings or conversation name.");
                    count++;
                    continue;
                }

                warmUpResponseBuilder.AppendLine(npcGlobalSettings.GlobalPromptPrefix);
                warmUpResponseBuilder.AppendLine();
                warmUpResponseBuilder.AppendLine(npc.Personality);
                warmUpResponseBuilder.AppendLine();
                warmUpResponseBuilder.AppendLine(npcGlobalSettings.GlobalPromptSuffix);

                prewarmData.Add(npc.ConversationName, warmUpResponseBuilder.ToString());

                count++;
            }

            timestamp = $"[{Time.time:F3}]";
            Debug.Log($"Prewarm(): {timestamp} Finished putting {count} NPCs into prewarm data.");

            await gemmaManager.Prewarm(prewarmData, (conversation, state) => {
                var oldState = initialized[conversation];
                initialized[conversation] = state;
                Debug.Log($"Prewarm(): {Time.time:F3} {conversation} {oldState.ToString()} -> {state.ToString()} ");
            });
            stateInputManager.AddInput(new InputLoaded());
        }
    }
}
