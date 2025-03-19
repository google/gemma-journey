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

using UnityEngine;
namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
  public abstract class StateWithTransition<StateMachineType, StateMachineViewHolderType> : State<StateMachineType, StateMachineViewHolderType> where StateMachineType : StateMachineWithTransition<StateMachineType, StateMachineViewHolderType> where StateMachineViewHolderType : IStateMachineViewHolder
  {
    public enum UpdateStep
    {
      TransitionIn, Update, TransitionOut
    }

    private UpdateStep _updateStep;
    public UpdateStep CurrentUpdateStep => _updateStep;

    public override void EnterState()
    {
      InternalEnterState();
      _updateStep = UpdateStep.TransitionIn;
    }
    protected abstract void InternalEnterState();
    public override void UpdateState()
    {
      if (_updateStep == UpdateStep.TransitionIn)
      {
        if (TransitionIn())
        {
          TransitionInDone();
        }
      }
      else if (_updateStep == UpdateStep.Update)
      {
        InternalUpdateState();
      }
      else if (_updateStep == UpdateStep.TransitionOut)
      {
        if (TransitionOut())
        {
          TransitionOutDone();
        }
      }

    }

    protected abstract bool TransitionIn();
    protected abstract void InternalUpdateState();
    protected abstract bool TransitionOut();
    public override void ExitState()
    {
      _updateStep = UpdateStep.TransitionOut;
      InternalExitState();
    }
    protected abstract void InternalExitState();
    public virtual void TransitionInDone()
    {
      _updateStep = UpdateStep.Update;
      Debug.LogFormat("State {0} Successful enter", GetType());
      _stateMachine.StateSuccessfullyEnter();
    }

    public virtual void TransitionOutDone()
    {
      _updateStep = UpdateStep.TransitionIn;
      Debug.LogFormat("State {0} Successful exit", GetType());
      _stateMachine.StateSuccessfullyExit();
    }
  }
}
