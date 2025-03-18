using System;
using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    [Serializable]
    public class StateChangeScene : ClueGameState
    {
        [Inject] private LevelManager _levelManager;
        [Inject] private UI.LoadingUserInterface _loadingUserInterface;
        [Inject] private UI.ControlOverlayUserInterface _controlOverlayUserInterface;

        protected override void InternalEnterState()
        {
            _levelManager.ChangeLevel();
            _levelManager.LoadNextLevel();
            _loadingUserInterface.Open();
            _controlOverlayUserInterface.ToggleControlButton(Vector2.zero);
        }

        protected override void InternalExitState()
        {
            _loadingUserInterface.Close();
        }

        public override void UpdateState()
        {
            if (_inputManager.HasInput<InputLevelLoaded>())
            {
                var input = _inputManager.GetFirstInput<InputLevelLoaded>();
                switch (input.LevelType)
                {
                    case LevelManager.LevelType.Ending:
                        StateMachineChangeStateTo(_stateMachine.StateEndgame);
                        break;
                    case LevelManager.LevelType.Level:
                        StateMachineChangeStateTo(_stateMachine.StateWalkAround);
                        break;
                }
            }
            base.UpdateState();
        }

        protected override void InternalUpdateState()
        {

        }

        protected override bool TransitionIn()
        {
            if (!_loadingUserInterface.IsReady)
            {
                return false;
            }
            return true;
        }

        protected override bool TransitionOut()
        {
            if (!_loadingUserInterface.IsReady)
            {
                return false;
            }
            return true;
        }
    }
}