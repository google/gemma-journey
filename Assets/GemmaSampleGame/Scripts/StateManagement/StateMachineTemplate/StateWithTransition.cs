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
      _stateMachine.StateSuccessfullyEnter();
    }

    public virtual void TransitionOutDone()
    {
      _updateStep = UpdateStep.TransitionIn;
      _stateMachine.StateSuccessfullyExit();
    }
  }
}
