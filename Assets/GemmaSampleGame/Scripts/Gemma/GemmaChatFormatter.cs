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
 * Utility class for formatting chat messages according to Gemma model's expected format.
 * 
 * This static class provides methods to format user messages, model responses,
 * system prompts, and complete conversation turns with the appropriate
 * turn delimiters required by the Gemma API.
 */
namespace GoogleDeepMind.GemmaSampleGame
{
    public static class GemmaChatFormatter
{
    /** Start delimiter for user messages in Gemma chat format */
    public const string USER_START = "<start_of_turn>user\n";
    
    /** Start delimiter for model responses in Gemma chat format */
    public const string MODEL_START = "<start_of_turn>model\n";
    
    /** End delimiter for any message turn in Gemma chat format */
    public const string TURN_END = "<end_of_turn>\n";

    /**
     * Formats a user message with appropriate Gemma chat delimiters.
     * 
     * @param message The raw user message content to format
     * @return The formatted user message with delimiters
     */
    public static string FormatUserMessage(string message)
    {
        return $"{USER_START}{message}{TURN_END}";
    }

    /**
     * Formats a model response with appropriate Gemma chat delimiters.
     * 
     * @param response The raw model response content to format
     * @return The formatted model response with delimiters
     */
    public static string FormatModelResponse(string response)
    {
        return $"{MODEL_START}{response}{TURN_END}";
    }

    /**
     * Formats a system prompt with appropriate Gemma chat delimiters.
     * System prompts are formatted as user messages followed by the start of a model response.
     * 
     * @param systemPrompt The system prompt content to format
     * @return The formatted system prompt with delimiters
     */
    public static string FormatSystemPrompt(string systemPrompt)
    {
        return $"{USER_START}{systemPrompt}{TURN_END}{MODEL_START}";
    }

    /**
     * Formats a complete conversation turn with user message and model response.
     * 
     * @param userMessage The user message content
     * @param modelResponse The model response content
     * @return The formatted conversation turn with both messages and appropriate delimiters
     */
    public static string FormatConversationTurn(string userMessage, string modelResponse)
    {
        return $"{FormatUserMessage(userMessage)}{FormatModelResponse(modelResponse)}";
    }

    /**
     * Returns just the model start delimiter.
     * 
     * @return The model start delimiter string
     */
    public static string FormatModelStart()
    {
        return MODEL_START;
    }
}
}