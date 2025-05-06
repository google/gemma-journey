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