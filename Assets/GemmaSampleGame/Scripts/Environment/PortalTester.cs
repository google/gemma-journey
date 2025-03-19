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