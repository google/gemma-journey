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
 * StateMachine.cs
 * 
 * Generic base class for implementing the state machine pattern.
 * Provides the core functionality for managing states, transitions between states,
 * and handling input events that drive state changes.
 * 
 * This class is designed to be extended by specific state machine implementations
 * that define their own states and transition rules.
 */
using System;
using UnityEngine;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
  public abstract class StateMachine<StateMachineType, StateMachineViewHolderType> where StateMachineType : StateMachine<StateMachineType, StateMachineViewHolderType> where StateMachineViewHolderType : IStateMachineViewHolder
  {
    /** The current active state in the state machine */
    protected State<StateMachineType, StateMachineViewHolderType> _currentState;

    /** Public accessor for the current state */
    public State<StateMachineType, StateMachineViewHolderType> CurrentState => _currentState;

    /** Manager for input events that drive state transitions */
    public StateInputManager InputManager { get; protected set; }

    /** Reference to the view holder that connects the state machine to UI components */
    [NonSerialized]
    public StateMachineViewHolderType View;

    /**
     * Constructor that initializes the input manager.
     */
    public StateMachine()
    {
      InputManager = new StateInputManager();
    }

    /**
     * Initializes the state machine with its view holder and sets up all states.
     * 
     * @param view The view holder that connects the state machine to UI components
     */
    public virtual void InitStateMachine(StateMachineViewHolderType view)
    {
      this.View = view;
      try
      {
        InitStates();
      }
      catch (NullReferenceException nre)
      {
        Debug.LogError("Please make sure that every state is Serializable by add [Serializable] on state implemented class");
        Debug.LogException(nre);
        throw nre;
      }
    }

    /**
     * Abstract method that must be implemented to initialize all states
     * and define valid transitions between them.
     */
    protected abstract void InitStates();

    /**
     * Abstract method that must be implemented to start the state machine
     * with its initial state.
     */
    public abstract void StartMachine();

    /**
     * Abstract method that must be implemented to stop the state machine
     * and perform any necessary cleanup.
     */
    public abstract void StopMachine();

    /**
     * Changes the current state from a source state to a destination state.
     * Validates that the transition is allowed and performs the state change
     * by calling exit and enter methods on the respective states.
     * 
     * @param fromState The source state to transition from
     * @param toState The destination state to transition to
     * @return True if the state change was successful
     * @throws ChangeStateFailedException if the transition is invalid
     */
    public virtual bool ChangeStateSourceToDestination(State<StateMachineType, StateMachineViewHolderType> fromState, State<StateMachineType, StateMachineViewHolderType> toState)
    {
      if (fromState == toState)
      {
        throw new ChangeStateFailedException(string.Format("Change state failed in state {0}: Self enter not support", fromState.GetType()));
      }
      if (_currentState != fromState)
      {
        throw new ChangeStateFailedException(string.Format("Change state failed caused by current state is {0} which is not {1}", _currentState.GetType(), fromState.GetType()));
      }
      if (!fromState.HasDestination(toState))
      {
        throw new ChangeStateFailedException(string.Format("Change state failed caused by state {0} has no transition to state {1}", fromState.GetType(), toState.GetType()));
      }
      if (_currentState == fromState && fromState.HasDestination(toState))
      {
        _currentState.ExitState();
        toState.EnterState();
        _currentState = toState;
        return true;
      }
      throw new ChangeStateFailedException("Change state failed with unknown reason");
    }

    /**
     * Updates the state machine each frame.
     * Processes new inputs and updates the current state.
     */
    public virtual void UpdateStateMachine()
    {
      InputManager.SwapInput();
      _currentState.UpdateState();
    }
  }
}
