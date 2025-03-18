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