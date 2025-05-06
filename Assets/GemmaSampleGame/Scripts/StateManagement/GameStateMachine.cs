/**
 * GameStateMachine.cs
 * 
 * Main state machine that controls the overall game flow for the Clue demo.
 * Inherits from StateMachineWithTransition and manages transitions between
 * different game states like menu, tutorial, dialogue, etc.
 */
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    public partial class GameStateMachine : StateMachineWithTransition<GameStateMachine, GameLifetimeScope>
    {
        [SerializeField]
        private UnityEvent<ClueGameState> StateChanged;

        /**
         * Initializes and starts the state machine with the starting state.
         * Called when the game begins.
         */
        public override void StartMachine()
        {
            _currentState = StateStart;
            _currentState.EnterState();
        }

        /**
         * Stops the state machine.
         * Currently not implemented but could be used for cleanup.
         */
        public override void StopMachine()
        {
            // Not currently implemented
        }

        /**
         * Updates the state machine each frame.
         * Handles global inputs like sound toggle and quit commands.
         */
        public override void UpdateStateMachine()
        {
            base.UpdateStateMachine();

            // Handle sound toggle input
            if (InputManager.HasInput<InputSound>())
            {
                Debug.Log("Toggle sound");
                bool enabled = InputManager.GetFirstInput<InputSound>().Enabled;
                if (enabled)
                {
                    AudioListener.pause = false;
                    AudioListener.volume = 1;
                }
                else
                {
                    AudioListener.pause = true;
                    AudioListener.volume = 0;
                }
            }

            // Handle quit game input
            if (InputManager.HasInput<InputQuit>())
            {
#if UNITY_EDITOR
                // If we are running in the Unity Editor
                Debug.Log("Quitting Play Mode in Editor...");
                EditorApplication.isPlaying = false;
#else
                Debug.Log("Quit game");
                Application.Quit();
#endif
            }
        }

        /**
         * Called when a state is successfully entered.
         * Broadcasts the state change event to listeners.
         */
        public override void StateSuccessfullyEnter()
        {
            base.StateSuccessfullyEnter();
            StateChanged?.Invoke(CurrentState as ClueGameState);
        }

        /**
         * Checks if the current state matches the specified state.
         * 
         * @param state The state to check against
         * @return True if the state matches, false otherwise
         */
        public bool IsInState(ClueGameState state)
        {
            return _currentState == state;
        }
    }
}
