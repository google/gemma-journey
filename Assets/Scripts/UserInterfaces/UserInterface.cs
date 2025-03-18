/**
 * Base class for all user interface components in the game.
 * 
 * Provides common functionality for creating, displaying, and hiding UI elements
 * using Unity's UI Toolkit. Specific UI screens should inherit from this class
 * and implement their own behavior.
 */
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace GoogleDeepMind.GemmaSampleGame.UI
{
    public class UserInterface : MonoBehaviour
    {
        [Header("UIDocument parameters")]
        /** Visual Tree Asset containing the UI layout */
        [SerializeField] protected VisualTreeAsset asset;

        /** Panel settings for rendering the UI */
        [SerializeField] protected PanelSettings panelSettings;

        /** Sorting order for this UI relative to other UIs */
        [SerializeField] protected int sortOrder;

        /** Flag indicating if the UI is fully initialized and ready for use */
        protected bool isReady = false;

        /**
         * Gets whether the UI is ready for interaction.
         * Can be overridden by derived classes to provide custom readiness logic.
         */
        public virtual bool IsReady => isReady;

        /** Root visual element of the UI hierarchy */
        protected VisualElement root;

        /**
         * Creates the UI when the GameObject is initialized.
         * Called during the Awake phase.
         */
        protected virtual void Awake()
        {
            CreateUserInterface();
        }

        /**
         * Initializes the UI and hides it by default.
         * Called during the Start phase.
         */
        protected virtual void Start()
        {
            Close();
        }

        /**
         * Called when the GameObject is enabled.
         * Can be overridden by derived classes to set up event subscriptions.
         */
        protected virtual void OnEnable()
        {
        }

        /**
         * Called when the GameObject is disabled.
         * Can be overridden by derived classes to clean up event subscriptions.
         */
        protected virtual void OnDisable()
        {
        }

        /**
         * Creates the UI Document and attaches it to a child GameObject.
         * Sets up the root visual element.
         */
        protected virtual void CreateUserInterface()
        {
            GameObject gameObject = new GameObject("UserInterface");
            gameObject.transform.parent = transform;
            UIDocument uiDocument = gameObject.AddComponent<UIDocument>();
            uiDocument.visualTreeAsset = asset;
            uiDocument.panelSettings = panelSettings;
            uiDocument.sortingOrder = sortOrder;
            root = uiDocument.rootVisualElement;
        }

        /**
         * Makes the UI visible to the user.
         */
        public virtual void Open()
        {
            root.style.display = DisplayStyle.Flex;
        }

        /**
         * Hides the UI from the user.
         */
        public virtual void Close()
        {
            root.style.display = DisplayStyle.None;
        }
    }
}
