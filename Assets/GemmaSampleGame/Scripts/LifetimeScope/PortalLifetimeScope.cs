using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GoogleDeepMind.GemmaSampleGame
{
    /// <summary>
    /// Lifetime scope for registering portal-related dependencies.
    /// </summary>
    public class PortalLifetimeScope : LifetimeScope
    {
        [SerializeField] private PortalManager portalManager;

        protected override void Configure(IContainerBuilder builder)
        {
            // Register the PortalManager as a singleton
            if (portalManager != null)
            {
                builder.RegisterComponent(portalManager).AsSelf();
            }
            else
            {
                Debug.LogError("PortalManager reference is missing in PortalLifetimeScope!");
            }
        }
    }
}
