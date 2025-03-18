/**
 * GemmaClient.cs
 * 
 * Client for interacting with Google's Gemma language model API.
 * Handles conversation management, message formatting, and communication
 * with the Gemma API for AI-powered NPC dialogues.
 */
using GemmaCpp;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class GemmaClient : MonoBehaviour
    {
        /** Default fallback response when no valid model response is available */
        private const string DEFAULT_NPC_RESPONSE = "Understood.";

        /** Reference to the Gemma API manager */
        [SerializeField]
        private GemmaManager gemmaManager;

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

        /** Delegate for response completion callback */
        public delegate void ResponseComplete(string message);

        /** Event fired when a prompt is sent to the model (for debugging) */
        public static event Action<GemmaClient, string> OnPrompt;

        /** Event fired when a response is received from the model (for debugging) */
        public static event Action<GemmaClient, string> OnResponse;

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
            systemPrompts.Add(new ChatMessage(ChatRole.Model, DEFAULT_NPC_RESPONSE));
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
        public async void Chat(string message, Gemma.TokenCallback onResponse, ResponseComplete onResponseComplete = null)
        {
            promptBuilder.Clear();
            // Add system prompt
            systemPrompts.ForEach((chat) =>
            {
                promptBuilder.Append(FormatChat(chat));
            });
            conversation.ForEach((chat) =>
            {
                promptBuilder.Append(FormatChat(chat));
            });

            // Add the new message
            string formattedPrompt = GemmaChatFormatter.FormatUserMessage(message) + GemmaChatFormatter.FormatModelStart();

            promptBuilder.Append(formattedPrompt);
            if (debugPrompt)
            {
                OnPrompt?.Invoke(this, promptBuilder.ToString());
                Debug.Log($"Prompt: {promptBuilder.ToString()}");
            }

            // Send to Gemma
            responseBuilder.Clear();
            string fullResponse = await gemmaManager.GenerateResponseAsync(promptBuilder.ToString(), (token) =>
            {
                responseBuilder.Append(token);
                if (onResponse != null)
                {
                    return onResponse(token);
                }
                return true;
            });

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
