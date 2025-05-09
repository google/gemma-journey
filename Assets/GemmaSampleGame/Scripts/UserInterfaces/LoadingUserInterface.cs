using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    public class LoadingUserInterface : UserInterface
    {
        [Header("Loading parameters")]
        [SerializeField] private Texture2D[] backgroundImages;
        [SerializeField] private float frameRate = 30f;

        private VisualElement _background;
        private TransitionHolder _transitionHolder;

        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        public void Construct(
            TransitionHolder transitionHolder
            )
        {
            _transitionHolder = transitionHolder;
        }

        protected override void Start()
        {
            root.styleSheets.Add(_transitionHolder.StyleSheet);
            root.AddToClassList(_transitionHolder.TransitionShort);
            root.AddToClassList(_transitionHolder.Fade);
            isReady = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _background = root.Q<VisualElement>("container");

            root.RegisterCallback<TransitionEndEvent>(HandleTransitionEndEvent);
            root.RegisterCallback<TransitionCancelEvent>(HandleTransitionCancelEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            root.UnregisterCallback<TransitionEndEvent>(HandleTransitionEndEvent);
            root.UnregisterCallback<TransitionCancelEvent>(HandleTransitionCancelEvent);
        }
        public override void Open()
        {
            isReady = false;
            root.RemoveFromClassList(_transitionHolder.Fade);
            _cancellationTokenSource = new CancellationTokenSource();
            PlayLoading(_cancellationTokenSource.Token);
        }

        public override void Close()
        {
            isReady = false;
            if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
            {
                _cancellationTokenSource.Cancel();
            }
            root.AddToClassList(_transitionHolder.Fade);
        }

        private void HandleTransitionEndEvent(TransitionEndEvent transition)
        {
            isReady = true;
        }
        private void HandleTransitionCancelEvent(TransitionCancelEvent transition)
        {
            isReady = true;
        }

        private async void PlayLoading(CancellationToken cancellationToken)
        {
            int delayMS = Mathf.RoundToInt(1000 / frameRate);
            int index = 0;
            if (backgroundImages.Length == 0)
            {
                return;
            }
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _background.style.backgroundImage = backgroundImages[index];
                    index = (index + 1) % backgroundImages.Length;
                    await UniTask.Delay(delayMS, cancellationToken: cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }
}