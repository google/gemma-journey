/**
 * ControlOverlayUserInterface.cs
 * 
 * User interface for the in-game control overlay that displays button hints,
 * legal links, sound toggle, and exit button. This class provides visual feedback 
 * for player input by highlighting WASD keys based on movement direction.
 * 
 * The overlay can show/hide different sections independently and provides
 * access to legal information like privacy policy and license information.
 */
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    public class ControlOverlayUserInterface : UserInterface
    {
        /** Button for exiting the game */
        private Button exitButton;

        /** Button for toggling sound on/off */
        private Button soundButton;

        /** Container for WASD control indicators */
        private VisualElement control;

        /** Container for legal text and links */
        private VisualElement textContainer;

        /** Label for privacy policy link */
        private Label privacyPolicy;

        /** Label for terms of use link */
        private Label termOfUse;

        /** Label for open source licenses link */
        private Label ossLicense;

        /** Visual element for the W (up) control button */
        private VisualElement wButton;

        /** Visual element for the A (left) control button */
        private VisualElement aButton;

        /** Visual element for the S (down) control button */
        private VisualElement sButton;

        /** Visual element for the D (right) control button */
        private VisualElement dButton;

        /** Visual element for the E (interact) control button */
        private VisualElement eButton;

        /** Reference to the state input manager for sending game inputs */
        private StateManagement.StateInputManager _stateInputManager;

        /** Reference to the transition holder for animation effects */
        private TransitionHolder _transitionHolder;

        /** Reference to game settings for URLs and configuration */
        private GameSettings _gameSettings;

        /** CSS class name for highlighting active control buttons */
        private const string controlHighlight = "control-highlight";

        /**
         * Dependency injection method to receive required references.
         * 
         * @param settings Game settings containing URLs and configuration
         * @param stateInputManager Reference to the state input manager
         * @param transitionHolder Reference to the transition effect holder
         */
        [Inject]
        public void Construct(
            GameSettings settings,
            StateManagement.StateInputManager stateInputManager,
            UI.TransitionHolder transitionHolder
            )
        {
            _gameSettings = settings;
            _stateInputManager = stateInputManager;
            _transitionHolder = transitionHolder;
        }

        /**
         * Initializes the UI when starting.
         * Sets up transitions and styles.
         */
        protected override void Start()
        {
            base.Start();
            root.styleSheets.Add(_transitionHolder.StyleSheet);
            root.AddToClassList(_transitionHolder.TransitionShort);
        }

        /**
         * Sets up UI elements and registers event callbacks when the UI is enabled.
         */
        protected override void OnEnable()
        {
            base.OnEnable();
            exitButton = root.Q<Button>("button-exit");
            exitButton.RegisterCallback<ClickEvent>(HandleExit);
            soundButton = root.Q<Button>("button-sound");
            soundButton.RegisterCallback<ClickEvent>(HandleSound);
            textContainer = root.Q<VisualElement>("text-container");
            control = root.Q<VisualElement>("control");

            privacyPolicy = root.Q<Label>("privacy-policy");
            privacyPolicy.RegisterCallback<ClickEvent>((_) =>
            {
                OpenUrl(_gameSettings.PrivacyPolicyUrl);
            });

            termOfUse = root.Q<Label>("terms-of-use");
            termOfUse.RegisterCallback<ClickEvent>((_) =>
            {
                OpenUrl(_gameSettings.TermsOfUseUrl);
            });

            ossLicense = root.Q<Label>("oss");
            ossLicense.RegisterCallback<ClickEvent>((_) =>
            {
                OpenUrl(_gameSettings.LicenseUrl);
            });


            wButton = root.Q<VisualElement>("w-button");
            aButton = root.Q<VisualElement>("a-button");
            sButton = root.Q<VisualElement>("s-button");
            dButton = root.Q<VisualElement>("d-button");
            eButton = root.Q<VisualElement>("e-button");

            root.RegisterCallback<TransitionEndEvent>(HandleTransitionEndEvent);
            root.RegisterCallback<TransitionCancelEvent>(HandleTransitionCancelEvent);
        }

        /**
         * Unregisters event callbacks when the UI is disabled.
         */
        protected override void OnDisable()
        {
            base.OnDisable();
            exitButton.UnregisterCallback<ClickEvent>(HandleExit);
            soundButton.UnregisterCallback<ClickEvent>(HandleSound);
        }

        /**
         * Opens the control overlay UI with a fade-in animation.
         */
        public override void Open()
        {
            root.RemoveFromClassList(_transitionHolder.Fade);
        }

        /**
         * Closes the control overlay UI with a fade-out animation.
         */
        public override void Close()
        {
            root.AddToClassList(_transitionHolder.Fade);
        }

        /**
         * Handles transition end events by marking the UI as ready.
         * 
         * @param transitionEvent The transition event data
         */
        protected void HandleTransitionEndEvent(TransitionEndEvent transitionEvent)
        {
            isReady = true;
        }

        /**
         * Handles transition cancel events by marking the UI as ready.
         * 
         * @param transitionEvent The transition cancel event data
         */
        protected void HandleTransitionCancelEvent(TransitionCancelEvent transitionEvent)
        {
            isReady = true;
        }

        /**
         * Handles exit button clicks by sending a quit input.
         * 
         * @param click The click event data
         */
        private void HandleExit(ClickEvent click)
        {
            _stateInputManager.AddInput(new StateManagement.InputQuit());
        }

        /**
         * Handles sound button clicks by toggling the sound state.
         * 
         * @param click The click event data
         */
        private void HandleSound(ClickEvent click)
        {
            soundButton.ToggleInClassList("button-sound-active");
            _stateInputManager.AddInput(new StateManagement.InputSound(soundButton.ClassListContains("button-sound-active")));
        }

        /**
         * Toggles the visibility of a UI element with animation.
         * 
         * @param element The element to toggle
         * @param enable Whether to show (true) or hide (false) the element
         */
        private void Toggle(VisualElement element, bool enable)
        {
            element.AddToClassList(_transitionHolder.TransitionShort);
            if (enable)
            {
                element.RemoveFromClassList(_transitionHolder.SlideDownFade);
            }
            else
            {
                element.AddToClassList(_transitionHolder.SlideDownFade);
            }
        }

        /**
         * Updates the control button highlights based on movement direction.
         * Highlights WASD buttons corresponding to the input direction vector.
         * 
         * @param direction The movement direction vector
         */
        public void ToggleControlButton(Vector2 direction)
        {
            if (direction.x > 0)
            {
                if (!dButton.ClassListContains(controlHighlight))
                {
                    dButton.AddToClassList(controlHighlight);
                }
            }
            if (direction.x < 0)
            {
                if (!aButton.ClassListContains(controlHighlight))
                {
                    aButton.AddToClassList(controlHighlight);
                }
            }

            if (direction.x == 0)
            {
                if (dButton.ClassListContains(controlHighlight))
                {
                    dButton.RemoveFromClassList(controlHighlight);
                }
                if (aButton.ClassListContains(controlHighlight))
                {
                    aButton.RemoveFromClassList(controlHighlight);
                }
            }
            if (direction.y > 0)
            {
                if (!wButton.ClassListContains(controlHighlight))
                {
                    wButton.AddToClassList(controlHighlight);
                }
            }
            if (direction.y < 0)
            {
                if (!sButton.ClassListContains(controlHighlight))
                {
                    sButton.AddToClassList(controlHighlight);
                }
            }

            if (direction.y == 0)
            {
                if (sButton.ClassListContains(controlHighlight))
                {
                    sButton.RemoveFromClassList(controlHighlight);
                }
                if (wButton.ClassListContains(controlHighlight))
                {
                    wButton.RemoveFromClassList(controlHighlight);
                }
            }
        }

        /**
         * Toggles the visibility of the control button section.
         * 
         * @param enable Whether to show (true) or hide (false) the controls
         */
        public void ToggleControl(bool enable)
        {
            Toggle(control, enable);
        }

        /**
         * Toggles the visibility of the text section with legal links.
         * 
         * @param enable Whether to show (true) or hide (false) the text
         */
        public void ToggleText(bool enable)
        {
            Toggle(textContainer, enable);
        }

        /**
         * Toggles the visibility of the sound button.
         * 
         * @param enable Whether to show (true) or hide (false) the sound button
         */
        public void ToggleSound(bool enable)
        {
            Toggle(soundButton, enable);
        }

        /**
         * Toggles the visibility of the exit button.
         * 
         * @param enable Whether to show (true) or hide (false) the exit button
         */
        public void ToggleExit(bool enable)
        {
            Toggle(exitButton, enable);
        }

        /**
         * Shows all UI elements in the control overlay.
         */
        public void ShowAll()
        {
            ToggleControl(true);
            ToggleText(true);
            ToggleSound(true);
            ToggleExit(true);
        }

        /**
         * Hides all UI elements in the control overlay.
         */
        public void HideAll()
        {
            ToggleControl(false);
            ToggleText(false);
            ToggleSound(false);
            ToggleExit(false);
        }

        /**
         * Opens a URL in the default web browser.
         * 
         * @param url The URL to open
         */
        private void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            Application.OpenURL(url);
        }
    }
}