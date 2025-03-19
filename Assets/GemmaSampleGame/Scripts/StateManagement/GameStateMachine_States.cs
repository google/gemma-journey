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
 * Implementation of the game state machine and state transitions for the Clue game.
 * 
 * This file contains the partial implementation of GameStateMachine that defines
 * all available states and their valid transitions. It also defines the base
 * ClueGameState class that all specific game states inherit from.
 */
using System;
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    using ClueStateSet = System.Collections.Generic.HashSet<State<GameStateMachine, GameLifetimeScope>>;
    [Serializable]
    public partial class GameStateMachine
    {
        /** State representing the initial game startup */
        public StateStart StateStart;

        /** State representing the main menu interface */
        public StateMenu StateMenu;

        /** State representing the tutorial sequence */
        public StateTutorial StateTutorial;

        /** State representing the player walking around the game environment */
        public StateWalkAround StateWalkAround;

        /** State representing active dialogue with an NPC */
        public StateDialogue StateDialogue;

        /** State representing the game ending sequence */
        public StateEndgame StateEndgame;

        /**
         * Initializes all states and defines their valid transitions.
         * This method sets up the directed graph of state transitions.
         */
        protected override void InitStates()
        {
            StateStart.InitState(this, new ClueStateSet() { StateMenu });
            StateMenu.InitState(this, new ClueStateSet() { StateTutorial, StateWalkAround });
            StateTutorial.InitState(this, new ClueStateSet() { StateWalkAround });
            StateWalkAround.InitState(this, new ClueStateSet() { StateDialogue, StateMenu, StateEndgame });
            StateDialogue.InitState(this, new ClueStateSet() { StateWalkAround, StateMenu });
            StateEndgame.InitState(this, new ClueStateSet() { StateMenu });
        }
    }

    /**
     * Base class for all Clue game states.
     * 
     * Provides common functionality for state initialization, 
     * entry/exit handling, and dependency injection for all game states.
     */
    public abstract class ClueGameState : StateWithTransition<GameStateMachine, GameLifetimeScope>
    {
        /**
         * Initializes the state with its parent state machine and valid destination states.
         * Also injects dependencies from the lifetime scope container.
         * 
         * @param stateMachine The parent state machine
         * @param destinationStates Set of states that can be transitioned to from this state
         */
        public override void InitState(GameStateMachine stateMachine, ClueStateSet destinationStates)
        {
            base.InitState(stateMachine, destinationStates);
            view.Container.Inject(this);
        }

        /**
         * Called when entering this state.
         * Logs the state transition and performs base class behavior.
         */
        public override void EnterState()
        {
            Debug.Log($"Entering state: {GetType()}");
            base.EnterState();
        }
    }
}