using System;
using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.StateManagement
{
    [Serializable]
    public class StateEndgame : ClueGameState
    {
        [Inject]
        private UI.EndingUserInterface endingHandler;
        [Inject]
        private LevelManager _levelManager;
        [Inject]
        private UI.LoadingUserInterface _loadingUserInterface;
        [Inject] 
        PlayerManager _playerManager;
        [Inject] private UI.BackdropUserInterface backdropUserInterface;

        protected override void InternalEnterState()
        {
            endingHandler.Open();
        }

        protected override void InternalExitState()
        {
            _playerManager.DespawnPlayer();

            _loadingUserInterface.Close();
        }

        public override void UpdateState()
        {
            Debug.Log($"Update end state");
            if (_inputManager.HasInput<InputPlay>())
            {
                Debug.Log("Input played");
                _levelManager.LoadStartScene();
                endingHandler.Close();
                _loadingUserInterface.Open();
                return;
            }
            if (_inputManager.HasInput<InputLevelLoaded>())
            {
                Debug.Log("Input level loaded");
                StateMachineChangeStateTo(_stateMachine.StateMenu);
                return;
            }
            base.UpdateState();
        }

        protected override void InternalUpdateState()
        {
            Debug.Log("Update internal end state");
        }

        protected override bool TransitionIn()
        {
            if (!endingHandler.IsReady)
            {
                return false;
            }
            return true;
        }
        public override void TransitionInDone()
        {
            base.TransitionInDone();
            backdropUserInterface.Open();
        }

        protected override bool TransitionOut()
        {
            if (!_loadingUserInterface.IsReady || !endingHandler.IsReady)
            {
                Debug.Log($"Update transition out end state loading: {_loadingUserInterface.IsReady}, End: {endingHandler.IsReady}");
                return false;
            }
            return true;
        }
    }
}