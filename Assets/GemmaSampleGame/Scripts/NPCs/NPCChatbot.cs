/**
 * NPCChatbot.cs
 * 
 * AI-powered NPC implementation that uses the Gemma language model.
 * Manages conversation state, animations, and interaction with the player.
 * Processes AI responses to update game state based on conversation content.
 */
using GemmaCpp;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Unity.Cinemachine;
using System.Text;
using GoogleDeepMind.GemmaSampleGame.StateManagement;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace GoogleDeepMind.GemmaSampleGame
{
    [RequireComponent(typeof(GemmaClient), typeof(PersonalityProvider))]
    public class NPCChatbot : MonoBehaviour, IConversational
    {
        /** Key for extracting secret word from AI response */
        private const string KEY_SECRET_WORD = "secret_word";
        /** Key for extracting message from AI response */
        private const string KEY_MESSAGE = "message";

        /** Reference to the NPC's animation handler */
        [SerializeField] private BotAnimationHandler _botAnimationHandler;

        /** Angle range in which the player is considered directly in front of the NPC */
        [SerializeField, Range(0, 180)] private float directAngleRange = 10;

        [SerializeField] private float stopTurnAngle = 30f;

        /** Turn speed */
        [SerializeField] private float turnSpeed;

        /** Camera for close-up shots during conversation */
        [SerializeField] private CinemachineCamera fancam;

        /** NPC personality provider */
        private PersonalityProvider personality;

        /** Reference to the Gemma API manager */
        private GemmaManager gemmaManager;

        /** User interface for chat display */
        private UI.ChatPanelUserInterface chatPanelManager;

        /** Client for communicating with Gemma */
        private GemmaClient gemmaClient;

        /** State input manager for game state transitions */
        private StateManagement.StateInputManager stateInputManager;

        /** Manager for secret information revealed by NPCs */
        private SecretManager secretManager;

        /** Global settings for all NPCs */
        private NPCGlobalSettings npcGlobalSettings;

        /** Chat history for this NPC */
        private List<UI.ChatPanelUserInterface.Chat> chatHistory = new();

        /** Current conversation state of the NPC */
        private ChatbotState currentState = ChatbotState.Initial;

        /** Whether the NPC has been initialized with first conversation */
        private bool isInitialized = false;

        /** Implementation of IConversational - returns the NPC's technical name */
        public string Name => gameObject.name;

        /** Implementation of IConversational - returns the NPC's display name */
        public string DisplayName => personality.Name;

        /**
         * Dependency injection constructor called by VContainer.
         * 
         * @param manager The Gemma API manager
         * @param stateInputManager The state input manager for game state transitions
         * @param secretManager The manager for secret information
         * @param dialogue The chat panel user interface
         * @param settings Global settings for all NPCs
         */
        [Inject]
        public void Construct(
            GemmaManager manager,
            StateManagement.StateInputManager stateInputManager,
            SecretManager secretManager,
            UI.ChatPanelUserInterface dialogue,
            NPCGlobalSettings settings)
        {
            this.gemmaManager = manager;
            this.stateInputManager = stateInputManager;
            this.secretManager = secretManager;
            this.npcGlobalSettings = settings;
            this.chatPanelManager = dialogue;
        }

        /**
         * Initializes the NPC's personality and Gemma client.
         */
        private void Awake()
        {
            personality = GetComponent<PersonalityProvider>();
            gemmaClient = GetComponent<GemmaClient>();
            gemmaClient.SetupGemmaManager(gemmaManager);
        }

        /**
         * Sets up initial state when the NPC is enabled.
         */
        private void Start()
        {
            secretManager.AddSecretWord(this, string.Empty);
            if (_botAnimationHandler != null)
            {
                _botAnimationHandler.Idle();
            }
            fancam.enabled = false;
        }

        private void Update()
        {
            if (stateInputManager.HasInput<InputDoor>())
            {
                if (stateInputManager.GetLastInput<InputDoor>().Type == InputDoor.ActionType.Enter)
                {
                    ResetConversation();
                }
            }
        }

        /**
         * Transitions the NPC to a new conversation state and updates UI/animations.
         * 
         * @param newState The state to transition to
         */
        private void TransitionTo(ChatbotState newState)
        {
            if (newState == currentState) return;

            var oldState = currentState;
            currentState = newState;

            Debug.Log($"Chatbot State: {oldState} -> {currentState}");

            // Handle state-specific UI updates
            switch (currentState)
            {
                case ChatbotState.Idle:
                    if (_botAnimationHandler != null)
                    {
                        _botAnimationHandler.Idle();
                    }
                    break;
                case ChatbotState.Thinking:
                    chatPanelManager.SetInputEnabled(false);
                    break;
                case ChatbotState.Listening:
                    chatPanelManager.SetInputEnabled(true);
                    if (_botAnimationHandler != null)
                    {
                        _botAnimationHandler.Listen();
                    }
                    break;
                case ChatbotState.Speaking:
                    if (_botAnimationHandler != null)
                    {
                        _botAnimationHandler.Speak();
                    }
                    break;
            }
        }

        /**
         * Called when the player interacts with this NPC.
         * Sets up the conversation UI and initiates dialogue.
         * 
         * @param gameObject The player game object
         */
        public void OnInteract(GameObject gameObject)
        {
            fancam.Lens = LensSettings.FromCamera(Camera.main);;
            fancam.enabled = true;

            stateInputManager.AddInput(new StateManagement.InputInteract(this));

            // Set UI elements
            chatPanelManager.SetNpcName(DisplayName);
            chatPanelManager.ClearChat();
            chatPanelManager.Open();

            // Chat
            TransitionTo(ChatbotState.Thinking);
            if (_botAnimationHandler != null)
            {
                TurnOvertime(gameObject.transform, this.GetCancellationTokenOnDestroy()).Forget();
            }
            if (!isInitialized)
            {
                //gemmaClient.Chat(personality.GetConversationPreset(0).PlayerMessage, HandleResponse, HandleFullResponse);
                gemmaClient.Chat("Hello!", HandleResponse, HandleFullResponse);
                isInitialized = true;
            }
            else
            {
                gemmaClient.Chat("Hello again!", HandleResponse, HandleFullResponse);
                chatPanelManager.AddChats(chatHistory);
            }
            _botAnimationHandler.Think();
            chatPanelManager.ShowThinking();
        }

        /** StringBuilder for accumulating model response tokens */
        private StringBuilder messageBuilder = new StringBuilder();

        /** Whether this is the first token in a response */
        private bool isFirstResponse = true;

        /** Whether currently parsing message content (vs. command content) */
        private bool isMessage = false;

        /**
         * Handles individual response tokens from the Gemma API.
         * Updates the chat UI as tokens are received.
         * 
         * @param response A single token from the model response
         * @return Whether to continue receiving tokens
         */
        public bool HandleResponse(string response)
        {
            TransitionTo(ChatbotState.Speaking);
            chatPanelManager.HideThinking();

            // Is this token is the separator then set and out
            if (response.Trim() == GetSeparator() && isMessage)
            {
                isMessage = false;
                return true;
            }

            // If this response is the first then clear the string builder and create new chat box
            if (isFirstResponse)
            {
                messageBuilder.Clear();
                isMessage = true;
                messageBuilder.Append(response);
                chatPanelManager.AddChat(UI.ChatPanelUserInterface.ChatType.Npc, messageBuilder.ToString());
                isFirstResponse = false;
            }
            else if (isMessage)
            {
                // Not first response but still a message
                messageBuilder.Append(response);
                chatPanelManager.EditChat(messageBuilder.ToString());
            }
            return true;
        }

        /**
         * Handles the complete response from the Gemma API.
         * Processes any commands embedded in the response to update game state.
         * 
         * @param response The complete model response
         */
        public void HandleFullResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                Debug.LogError("Response is null or empty");
                chatPanelManager.HideThinking();
                TransitionTo(ChatbotState.Speaking);
                TransitionTo(ChatbotState.Listening);
                return;
            }
            string[] responses = response.Split(GetSeparator(), 2, StringSplitOptions.RemoveEmptyEntries);
            chatHistory.Add(new UI.ChatPanelUserInterface.Chat(UI.ChatPanelUserInterface.ChatType.Npc, responses[0]));
            if (responses.Length < 2)
            {
                Debug.Log("Response does not contain other sections");
                TransitionTo(ChatbotState.Listening);
            }
            else
            {
                try
                {
                    Dictionary<string, string> commands = ParseCommand(responses[1]);

                    if (secretManager.GetSecretWord(this) == string.Empty && commands.ContainsKey(KEY_SECRET_WORD))
                    {
                        secretManager.AddSecretWord(this, commands[KEY_SECRET_WORD]);
                    }

                    if (commands.ContainsKey("correct"))
                    {
                        bool.TryParse(commands["correct"], out bool correct);
                        if (correct)
                        {
                            // Open the door
                            Debug.Log("Player is correct!!!!");
                            stateInputManager.AddInput(new StateManagement.InputNpcDone());
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }

            isFirstResponse = true;
            TransitionTo(ChatbotState.Listening);
            Debug.Log("NPC done response reply");
        }

        /**
         * Gets the separator string that divides visible message from command data.
         * 
         * @return The separator string
         */
        private string GetSeparator()
        {
            return "|";
        }

        /**
         * Parses command data from the model response.
         * 
         * @param commands JSON string containing commands
         * @return Dictionary of command key-value pairs
         */
        private Dictionary<string, string> ParseCommand(string commands)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(commands);
        }

        /**
         * Called when the player chooses to end the conversation.
         * Resets UI state and transitions back to the game world.
         */
        public void LeaveConversation()
        {
            Debug.Log($"[SimpleChatbot] Player chose to leave conversation with {personality.Name}");
            // Start the leaving sequence
            TransitionTo(ChatbotState.Idle);
            chatHistory.Add(new UI.ChatPanelUserInterface.Chat(UI.ChatPanelUserInterface.ChatType.Block, UI.ChatPanelUserInterface.END_CONVERSATION_BLOCK));
            chatPanelManager.SetInputEnabled(false);
            stateInputManager.AddInput(new StateManagement.InputDoneInteract());
            fancam.enabled = false;
            if (_botAnimationHandler != null)
            {
                _botAnimationHandler.Greet();
            }
        }

        /**
         * Processes a message from the player and sends it to the Gemma API.
         * 
         * @param message The player's message text
         */
        public void ListenPlayerMessage(string message)
        {
            if (currentState != ChatbotState.Listening)
            {
                Debug.Log($"[Chatbot] {personality.Name} not ready");
                return;
            }
            TransitionTo(ChatbotState.Thinking);
            gemmaClient.Chat(message, HandleResponse, HandleFullResponse);
            var chat = new UI.ChatPanelUserInterface.Chat(UI.ChatPanelUserInterface.ChatType.Player, message);
            chatPanelManager.AddChat(chat);
            chatHistory.Add(chat);
            if (_botAnimationHandler != null)
            {
                _botAnimationHandler.Think();
            }
            chatPanelManager.ShowThinking();
            chatPanelManager.ScrollToBottom();
        }

        /**
         * Gets the current conversation history as a formatted string.
         * 
         * @return The formatted conversation history
         */
        public string GetConversation()
        {
            return gemmaClient.GetConversation();
        }

        /**
         * Resets the conversation state and clears chat history.
         */
        public void ResetConversation()
        {
            chatHistory.Clear();
            chatPanelManager.ClearChat();
            //personality.ReloadPersonality();
            //gemmaClient.ClearConversation();
            chatPanelManager.SetNpcName(personality.Name);
            //gemmaClient.SetSystemPrompt(personality.SystemPrompt);
            //gemmaClient.WarmUp();
        }

        public async UniTask TurnOvertime(Transform target, CancellationToken cancellationToken)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            BotAnimationHandler.InteractionSide side;
            if (Mathf.Abs(angle) < directAngleRange)
            {
                side = BotAnimationHandler.InteractionSide.Direct;
            }
            else
            {
                if (angle < 0)
                {
                    side = BotAnimationHandler.InteractionSide.Left;
                }
                else
                {
                    side = BotAnimationHandler.InteractionSide.Right;
                }
            }
            _botAnimationHandler.Heard(side);
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (target != null)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                    );
                    float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
                    if (angleDifference < stopTurnAngle)
                    {
                        break;
                    }
                }
                await UniTask.Yield(cancellationToken);
            }
            _botAnimationHandler.Heard(BotAnimationHandler.InteractionSide.Direct);
        }
    }
}
