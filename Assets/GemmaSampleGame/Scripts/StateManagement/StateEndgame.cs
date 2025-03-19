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

        private bool _levelLoaded = false;
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
                _levelLoaded = true;
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