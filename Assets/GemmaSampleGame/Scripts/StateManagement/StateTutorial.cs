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
    public class StateTutorial : ClueGameState
    {
        [Inject] private UI.TutorialUserInterface tutorialUserInterface;
        [Inject] private UI.ControlOverlayUserInterface controlOverlayUserInterface;
        [Inject] private LevelManager levelManager;

        [Inject] private PlayerManager playerManager;
        protected override void InternalEnterState()
        {
            tutorialUserInterface.Open();
            controlOverlayUserInterface.ShowAll();
            controlOverlayUserInterface.Open();
            levelManager.LoadNextLevel();
        }

        protected override void InternalExitState()
        {
            tutorialUserInterface.Close();

            Debug.Log("This would be a good time to spawn the player.");
            playerManager.SpawnPlayer();
        }

        public override void UpdateState()
        {
            if (_inputManager.HasInput<InputPlay>() || _inputManager.HasInput<InputCancel>())
            {
                StateMachineChangeStateTo(_stateMachine.StateWalkAround);
                return;
            }
            base.UpdateState();
        }

        protected override void InternalUpdateState()
        {

        }

        protected override bool TransitionIn()
        {
            if (tutorialUserInterface.IsReady)
            {
                return true;
            }
            return false;
        }

        protected override bool TransitionOut()
        {
            if (tutorialUserInterface.IsReady)
            {
                return true;
            }
            return false;
        }

        public override void TransitionOutDone()
        {
            base.TransitionOutDone();
            _inputManager.AddInput(new InputDoor(InputDoor.ActionType.Open));
        }
    }
}