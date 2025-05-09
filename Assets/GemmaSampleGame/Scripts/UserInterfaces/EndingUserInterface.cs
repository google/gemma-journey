using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    public class EndingUserInterface : UserInterface
    {
        [Header("Ending parameters")]
        [SerializeField] private Texture2D[] backgroundImages;
        [SerializeField] private int stopIndex;
        [SerializeField] private float frameRate = 30f;
        [SerializeField] private float outFrameRateMultiplier = 4f;

        private int currentIndex;

        private Button _learnMoreButton;
        private Button _playAgainButton;
        private VisualElement _background;

        private StateManagement.StateInputManager _stateInputManager;
        private TransitionHolder _transitionHolder;
        private GameSettings _gameSettings;

        private bool _isTransitioning = false;

        private CancellationTokenSource _cancellationTokenSource;

        private float currentFrameRate = 30f;

        [Inject]
        public void Construct(
            GameSettings gameSettings,
            StateManagement.StateInputManager stateInputManager,
            TransitionHolder transitionHolder
            )
        {
            _gameSettings = gameSettings;
            _stateInputManager = stateInputManager;
            _transitionHolder = transitionHolder;
        }

        protected override void Start()
        {
            root.styleSheets.Add(_transitionHolder.StyleSheet);
            root.AddToClassList(_transitionHolder.TransitionShort);
            root.AddToClassList(_transitionHolder.Fade);
            isReady = true;
            currentFrameRate = frameRate;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _learnMoreButton = root.Q<Button>("learn-more-button");
            _learnMoreButton.RegisterCallback<ClickEvent>((_) =>
            {
                if (!string.IsNullOrEmpty(_gameSettings.LearnMoreUrl))
                {
                    Application.OpenURL(_gameSettings.LearnMoreUrl);
                }
            });

            _playAgainButton = root.Q<Button>("play-again-button");
            _playAgainButton.RegisterCallback<ClickEvent>(HandleStart);

            _background = root.Q<VisualElement>("container");

            root.RegisterCallback<TransitionStartEvent>(HandleTransitionStartEvent);
            root.RegisterCallback<TransitionEndEvent>(HandleTransitionEndEvent);
            root.RegisterCallback<TransitionCancelEvent>(HandleTransitionCancelEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _playAgainButton.UnregisterCallback<ClickEvent>(HandleStart);
            root.UnregisterCallback<TransitionStartEvent>(HandleTransitionStartEvent);
            root.UnregisterCallback<TransitionEndEvent>(HandleTransitionEndEvent);
            root.UnregisterCallback<TransitionCancelEvent>(HandleTransitionCancelEvent);
        }

        public override async void Open()
        {
            isReady = false;
            _isTransitioning = true;
            root.RemoveFromClassList(_transitionHolder.Fade);
            await UniTask.WaitUntil(() => !_isTransitioning);
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                currentFrameRate = frameRate;
                await PlayBackground(0, stopIndex, _cancellationTokenSource.Token);
                isReady = true;
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("Play background canceled");
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public override async void Close()
        {
            if (_cancellationTokenSource != null)
            {
                currentFrameRate = frameRate * outFrameRateMultiplier;
                _cancellationTokenSource.Cancel();
            }
            isReady = false;
            _cancellationTokenSource = new CancellationTokenSource();
            await PlayBackground(currentIndex, backgroundImages.Length, _cancellationTokenSource.Token);
            _isTransitioning = true;
            root.AddToClassList(_transitionHolder.Fade);
            await UniTask.WaitUntil(() => !_isTransitioning);
            isReady = true;
        }

        private void HandleStart(ClickEvent clickEvent)
        {
            _stateInputManager.AddInput(new StateManagement.InputPlay());
        }

        protected void HandleTransitionStartEvent(TransitionStartEvent transitionEvent)
        {
            Debug.Log("Transition start");
            _isTransitioning = true;
        }

        protected void HandleTransitionEndEvent(TransitionEndEvent transitionEvent)
        {
            _isTransitioning = false;
        }

        protected void HandleTransitionCancelEvent(TransitionCancelEvent transitionEvent)
        {
            _isTransitioning = false;
        }

        private async UniTask PlayBackground(int startIndex, int stopIndex, CancellationToken cancellationToken)
        {
            int delayMS = Mathf.RoundToInt(1000 / frameRate);
            for (int i = startIndex; i < stopIndex; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _background.style.backgroundImage = backgroundImages[i];
                currentIndex = i;
                await UniTask.Delay(delayMS, cancellationToken: cancellationToken);
            }
        }
    }
}