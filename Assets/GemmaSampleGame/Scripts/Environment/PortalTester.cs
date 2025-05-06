using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame
{
    /// <summary>
    /// A simple test script for the PortalManager functionality.
    /// This can be attached to a GameObject in the scene to test portal visibility and positioning.
    /// </summary>
    public class PortalTester : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleVisibilityKey = KeyCode.P;
        [SerializeField] private KeyCode repositionKey = KeyCode.R;

        private PortalManager portalManager;

        [Inject]
        public void Construct(PortalManager portalManager)
        {
            this.portalManager = portalManager;
        }

        private void Update()
        {
            // Toggle portal visibility
            if (Input.GetKeyDown(toggleVisibilityKey))
            {
                if (portalManager != null)
                {
                    portalManager.SetPortalVisibility(!portalManager.IsVisible);
                    Debug.Log($"Portal visibility toggled to: {portalManager.IsVisible}");
                }
            }

            // Reposition portal at current room
            if (Input.GetKeyDown(repositionKey))
            {
                if (portalManager != null)
                {
                    portalManager.PositionPortalAtCurrentRoom();
                    Debug.Log("Portal repositioned at current room's back anchor");
                }
            }
        }
    }
}