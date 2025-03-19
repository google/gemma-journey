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

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
  public abstract class State<StateMachineType, StateMachineViewHolderType> where StateMachineType : StateMachine<StateMachineType, StateMachineViewHolderType> where StateMachineViewHolderType : IStateMachineViewHolder
  {
    [NonSerialized]
    protected StateMachineType _stateMachine;
    protected StateMachineViewHolderType view => _stateMachine.View;
    protected StateInputManager _inputManager => _stateMachine.InputManager;
    [NonSerialized]
    protected HashSet<State<StateMachineType, StateMachineViewHolderType>> _destinationStates;
    public virtual void InitState(StateMachineType stateMachine, HashSet<State<StateMachineType, StateMachineViewHolderType>> destinationStates)
    {
      _stateMachine = stateMachine;
      _destinationStates = destinationStates;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public virtual bool HasDestination(State<StateMachineType, StateMachineViewHolderType> state)
    {
      return _destinationStates != null && _destinationStates.Contains(state);
    }

    public virtual bool StateMachineChangeStateTo(State<StateMachineType, StateMachineViewHolderType> state)
    {
      return _stateMachine.ChangeStateSourceToDestination(this, state);
    }

    /// <summary>
    /// Gets the set of available destination states that this state can transition to.
    /// </summary>
    /// <returns>A HashSet of destination states, or null if none are available.</returns>
    public virtual HashSet<State<StateMachineType, StateMachineViewHolderType>> GetDestinations()
    {
      return _destinationStates;
    }
  }
}
