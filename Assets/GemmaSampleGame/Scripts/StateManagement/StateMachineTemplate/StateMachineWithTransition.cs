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


namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
  public abstract class StateMachineWithTransition<StateMachineType, StateMachineViewHolderType> : StateMachine<StateMachineType, StateMachineViewHolderType> where StateMachineType : StateMachineWithTransition<StateMachineType, StateMachineViewHolderType> where StateMachineViewHolderType : IStateMachineViewHolder
  {
    protected State<StateMachineType, StateMachineViewHolderType> _destinationState;
    public State<StateMachineType, StateMachineViewHolderType> DestinationState => _destinationState;

    public override bool ChangeStateSourceToDestination(State<StateMachineType, StateMachineViewHolderType> fromState, State<StateMachineType, StateMachineViewHolderType> toState)
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
        InputManager.SetStateChangeInputs();
        _currentState.ExitState();
        _destinationState = toState;
        return true;
      }
      throw new ChangeStateFailedException("Change state failed with unknown reason");
    }

    public virtual void StateSuccessfullyEnter()
    {

    }

    public virtual void StateSuccessfullyExit()
    {
      _destinationState.EnterState();
      _currentState = _destinationState;
    }
  }
}