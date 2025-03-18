/**
 * UserInterfaceManager.cs
 * 
 * Central manager for all user interface components in the game.
 * This class handles the registration and initialization of UI components
 * with the dependency injection container, ensuring that all UIs receive
 * their required dependencies.
 * 
 * The manager finds all UserInterface components in its children and registers
 * them with the VContainer DI system during setup.
 */
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class UserInterfaceManager : MonoBehaviour
    {
        /** Whether to log detailed debug information about UI registration */
        [SerializeField] private bool debug = false;

        /** Holder for transition effects shared across UI components */
        [SerializeField] private UI.TransitionHolder transitionHolder;

        /**
         * Registers all child UserInterface components with the dependency injection container.
         * Called during the container build phase to make UI components available for injection.
         * 
         * @param builder The container builder to register components with
         */
        public void RegisterUserInterface(IContainerBuilder builder)
        {
            var uis = GetComponentsInChildren<UI.UserInterface>();
            foreach (var ui in uis)
            {
                if (debug)
                {
                    Debug.Log($"Register: {ui.name}");
                }
                builder.RegisterComponent(ui).AsSelf();
            }
            builder.RegisterComponent(transitionHolder);
        }

        /**
         * Initializes all child UserInterface components by injecting their dependencies.
         * Called after the container is built to provide dependencies to UI components.
         * 
         * @param container The object resolver to use for dependency injection
         */
        public void InitUserInterface(IObjectResolver container)
        {
            var uis = GetComponentsInChildren<UI.UserInterface>();
            foreach (var ui in uis)
            {
                if (debug)
                {
                    Debug.Log($"Inject: {ui.name}");
                }
                container.Inject(ui);
            }
        }
    }
}
