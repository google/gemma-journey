/**
 * GemmaClient.cs
 * 
 * Client for interacting with Google's Gemma language model API.
 * Handles conversation management, message formatting, and communication
 * with the Gemma API for AI-powered NPC dialogues.
 */
using Cysharp.Threading.Tasks;
using GemmaCpp;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using static GemmaCpp.GemmaManager;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class GemmaClient : MonoBehaviour
    {
        public enum GemmaClientState
        {
            None = 0,
            WarmingUp = 1,
            Ready = 2,
            Generating = 3,
            Responding = 4,
        }
        /** Default fallback response when no valid model response is available */
        private const string DEFAULT_NPC_RESPONSE = "Understood.";

        /** Reference to the Gemma API manager */
        [SerializeField]
        private GemmaManager gemmaManager;

        private NPCPrewarmer npcPrewarmer;

        /** Whether to save the conversation history */
        [SerializeField]
        private bool saveConversation = true;

        /** Whether to log prompts and responses for debugging */
        [SerializeField]
        private bool debugPrompt = false;

        /** StringBuilder for constructing prompts to the model */
        private StringBuilder promptBuilder = new();

        /** StringBuilder for collecting model response tokens */
        private StringBuilder responseBuilder = new();

        /** Conversation history between user and model */
        private List<ChatMessage> conversation = new List<ChatMessage>();

        /** System prompts that establish NPC personality and behavior */
        private List<ChatMessage> systemPrompts = new List<ChatMessage>();

        public IEnumerable<ChatMessage> SystemPrompts => systemPrompts;

        /** Delegate for response completion callback */
        public delegate void ResponseComplete(string message);

        /** Delegate for response failed callback */
        public delegate void ResponseFailed(string failedMessage);

        /** Event fired when a prompt is sent to the model (for debugging) */
        public static event Action<GemmaClient, string> OnPrompt;

        /** Event fired when a response is received from the model (for debugging) */
        public static event Action<GemmaClient, string> OnResponse;

        /** State of the gemma client */
        private GemmaClientState state = GemmaClientState.None;


        [Inject]
        public void Construct(GemmaManager gemmaManager, NPCPrewarmer npcPrewarmer)
        {
            Debug.Log("GemmaClient: GemmaManager injected");
            this.gemmaManager = gemmaManager;
            Debug.Log("GemmaClient: NPCPrewarmer injected");
            this.npcPrewarmer = npcPrewarmer;
        }

        /**
         * Sets up the Gemma manager reference.
         * 
         * @param gemmaManager The Gemma API manager instance
         */
        public void SetupGemmaManager(GemmaManager gemmaManager)
        {
            this.gemmaManager = gemmaManager;
        }

        /**
         * Sets the system prompt that establishes NPC personality and behavior.
         * Clears any existing system prompts and adds the new one.
         * 
         * @param prompt The system prompt text
         */
        public void SetSystemPrompt(string prompt)
        {
            systemPrompts.Clear();
            systemPrompts.Add(new ChatMessage(ChatRole.User, prompt));
        }

        /**
         * Adds a user-model message pair to the conversation history.
         * 
         * @param userMessage The user message
         * @param modelMessage The model's response message
         */
        public void AddConversation(ChatMessage userMessage, ChatMessage modelMessage)
        {
            if (userMessage == null)
            {
                return;
            }
            conversation.Add(userMessage);
            if (!string.IsNullOrEmpty(modelMessage.Content))
            {
                conversation.Add(modelMessage);
            }
            else
            {
                conversation.Add(new ChatMessage(ChatRole.Model, DEFAULT_NPC_RESPONSE));
            }
        }

        /**
         * Adds a user-model message pair to the conversation history using strings.
         * 
         * @param userMessage The user message text
         * @param modelMessage The model's response text (defaults to DEFAULT_NPC_RESPONSE)
         */
        public void AddConversation(string userMessage, string modelMessage = DEFAULT_NPC_RESPONSE)
        {
            conversation.Add(new ChatMessage(ChatRole.User, userMessage));
            conversation.Add(new ChatMessage(ChatRole.Model, modelMessage));
        }

        /**
         * Clears the conversation history.
         */
        public void ClearConversation()
        {
            conversation.Clear();
        }

        /**
         * Gets the full formatted conversation history as a string.
         * 
         * @return The formatted conversation history
         */
        public string GetConversation()
        {
            promptBuilder.Clear();
            systemPrompts.ForEach((chat) =>
            {
                promptBuilder.Append(FormatChat(chat));
            });
            conversation.ForEach((chat) =>
            {
                promptBuilder.Append(FormatChat(chat));
            });
            return promptBuilder.ToString();
        }

        /**
         * Sends a message to the Gemma API and processes the response.
         * 
         * @param message The user message to send
         * @param onResponse Callback for each token of the response
         * @param onResponseComplete Callback for when the full response is received
         */
        public async void Chat(string message, Gemma.TokenCallback onResponse, ResponseComplete onResponseComplete = null, ResponseFailed onResponseFailed = null)
        {
            if (state == GemmaClientState.None)
            {
                // check for prewarm
                var prewarm = npcPrewarmer.CheckPrewarmState(gameObject.name);

                if (prewarm == PrewarmState.NotApplicable)
                {
                    Debug.LogWarning("GemmaClient: not set up for prewarming");
                    return;
                }
                state = GemmaClientState.WarmingUp;
            }
            if (state == GemmaClientState.WarmingUp)
            {
                Debug.LogWarning("Is warming up, the message will be sent after the client is ready");
                await UniTask.WaitUntil(() =>
                {
                    var p = npcPrewarmer.CheckPrewarmState(gameObject.name);
                    return p == PrewarmState.Done;
                }, PlayerLoopTiming.Update);
                state = GemmaClientState.Ready;
            }

            if (state == GemmaClientState.Generating || state == GemmaClientState.Responding)
            {
                // If generating or responding, interrupt the process and send new message
                // Interrupt gemma here
                string failedMessage = $"GemmaClient is {state} message will be ignored for now";
                if (debugPrompt)
                {
                    Debug.LogError(failedMessage);
                }
                onResponseFailed?.Invoke(message);
                return;
            }

            state = GemmaClientState.Generating;

            if (debugPrompt)
            {
                OnPrompt?.Invoke(this, message);
                Debug.Log($"Prompt: {message}");
            }

            // Configure the conversation context
            var conversationContextName = gameObject.name;
            if (!gemmaManager.HasConversation(conversationContextName)) {
                gemmaManager.CreateConversation(conversationContextName);
            }
            gemmaManager.SwitchConversation(conversationContextName);

            // force multiturn on
            gemmaManager.SetMultiturn(true);

            // Send to Gemma
            responseBuilder.Clear();
            string fullResponse = await gemmaManager.GenerateResponseAsync(message, (token) =>
            {
                if (state != GemmaClientState.Responding)
                {
                    state = GemmaClientState.Responding;
                }
                responseBuilder.Append(token);
                if (onResponse != null)
                {
                    return onResponse(token);
                }
                return true;
            });

            fullResponse = responseBuilder.ToString();
            if (saveConversation)
            {
                conversation.Add(new ChatMessage(ChatRole.User, message));
                conversation.Add(new ChatMessage(ChatRole.Model, fullResponse));
            }
            if (debugPrompt)
            {
                OnResponse?.Invoke(this, fullResponse);
                Debug.Log($"Response: {fullResponse}");
            }
            onResponseComplete(fullResponse);
            state = GemmaClientState.Ready;
        }

        /**
         * Formats a chat message according to its role (user or model).
         * 
         * @param chatMessage The message to format
         * @return The formatted message string
         */
        private string FormatChat(ChatMessage chatMessage)
        {
            if (chatMessage.Role == ChatRole.User)
            {
                return GemmaChatFormatter.FormatUserMessage(chatMessage.Content);
            }
            else
            {
                return GemmaChatFormatter.FormatModelResponse(chatMessage.Content);
            }
        }
    }
}
