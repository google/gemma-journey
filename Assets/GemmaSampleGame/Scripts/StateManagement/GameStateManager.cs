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
 * GameStateManager.cs
 * 
 * Manages high-level game state transitions and tracking.
 * Provides a simplified state system compared to the more complex
 * StateMachine implementation used elsewhere.
 */
using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    /**
     * Defines the primary game states that control overall game flow.
     */
    public enum GameState
    {
        WalkingAround,  // Player exploring the environment
        Dialog,         // Player in conversation with an NPC
        Menu            // Player in menu screens
    }

    /**
     * Manages transitions between high-level game states and notifies
     * listeners of state changes via events.
     */
    public class GameStateManager
    {
        /** The current active game state */
        private GameState currentState;

        /** Public accessor for the current game state */
        public GameState CurrentState => currentState;

        /** Event fired when the game state changes */
        public event System.Action<GameState> OnStateChanged;

        /**
         * Constructor that initializes the game state to WalkingAround.
         */
        public GameStateManager()
        {
            TransitionTo(GameState.WalkingAround);  // Set initial state
        }

        /**
         * Transitions the game to a new state if it's different from the current state.
         * Notifies listeners via OnStateChanged event.
         * 
         * @param newState The state to transition to
         */
        public void TransitionTo(GameState newState)
        {
            if (newState == currentState) return;

            var oldState = currentState;
            currentState = newState;

            Debug.Log($"Game State: {oldState} -> {currentState}");
            OnStateChanged?.Invoke(currentState);
        }

        /**
         * Checks if the game is currently in the specified state.
         * 
         * @param state The state to check against
         * @return True if the current state matches, false otherwise
         */
        public bool IsInState(GameState state) => currentState == state;
    }
}