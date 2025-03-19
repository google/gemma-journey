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
using UnityEngine.Events;
using UnityEngine.UIElements;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    public class TutorialUserInterface : UserInterface
    {
        private Button startButton;
        private Button closeButton;

        private StateManagement.StateInputManager _stateInputManager;
        private UI.TransitionHolder _transitionHolder;

        [Inject]
        public void Construct(
            StateManagement.StateInputManager stateInputManager,
            UI.TransitionHolder transitionHolder
            )
        {
            _stateInputManager = stateInputManager;
            _transitionHolder = transitionHolder;
        }

        protected override void Start()
        {
            base.Start();
            root.styleSheets.Add(_transitionHolder.StyleSheet);
            root.AddToClassList(_transitionHolder.TransitionShort);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            startButton = root.Q<Button>("button-start");
            startButton.RegisterCallback<ClickEvent>(HandleStart);

            root.RegisterCallback<TransitionEndEvent>(HandleTransitionEnd);
            root.RegisterCallback<TransitionCancelEvent>(HandleTransitionCancel);
        }

        protected override void OnDisable()
        {
            startButton.UnregisterCallback<ClickEvent>(HandleStart);

            root.UnregisterCallback<TransitionEndEvent>(HandleTransitionEnd);
            root.UnregisterCallback<TransitionCancelEvent>(HandleTransitionCancel);
        }

        public override void Open()
        {
            isReady = false;
            root.RemoveFromClassList(_transitionHolder.SlideDownFade);
        }

        public override void Close()
        {
            isReady = false;
            root.AddToClassList(_transitionHolder.SlideDownFade);
        }
        private void HandleStart(ClickEvent click)
        {
            _stateInputManager.AddInput(new StateManagement.InputPlay());
        }

        private void HandleTransitionEnd(TransitionEndEvent transitionEndEvent)
        {
            isReady = true;
        }

        private void HandleTransitionCancel(TransitionCancelEvent transitionCancelEvent)
        {
            isReady = true;
        }
    }
}