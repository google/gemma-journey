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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GoogleDeepMind.GemmaSampleGame
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private Transform frontAnchor;
        [SerializeField] private Transform backAnchor;
        [SerializeField] private DoorManager doorManager;

        public Transform FrontAnchor => frontAnchor;
        public Transform BackAnchor => backAnchor;
        public DoorManager DoorManager => doorManager;

#if UNITY_EDITOR
        [SerializeField] private Color frontAnchorColor = Color.blue;
        [SerializeField] private Color backAnchorColor = Color.red;
        [SerializeField] private float gizmoSize = 0.5f;

        private void OnDrawGizmos()
        {
            if (frontAnchor != null)
            {
                Gizmos.color = frontAnchorColor;
                Gizmos.DrawSphere(frontAnchor.position, gizmoSize);
                Handles.color = frontAnchorColor;
                Handles.Label(frontAnchor.position + Vector3.up * gizmoSize, "Front Anchor");
            }

            if (backAnchor != null)
            {
                Gizmos.color = backAnchorColor;
                Gizmos.DrawSphere(backAnchor.position, gizmoSize);
                Handles.color = backAnchorColor;
                Handles.Label(backAnchor.position + Vector3.up * gizmoSize, "Back Anchor");
            }
        }
#endif

        private LevelManager levelManager;
        private PortalManager portalManager;
        [Inject]
        public void Construct(LevelManager levelManager, PortalManager portalManager)
        {
            this.levelManager = levelManager;
            this.portalManager = portalManager;
        }

        private void Start()
        {
            levelManager.RegisterRoom(this);
        }

        public void EnableRoom()
        {
            Debug.Log($"Enable room: {name}");
            portalManager.PositionPortalAtCurrentRoom();
        }

        public void DisableRoom()
        {
            Debug.Log($"Disable room: {name}");
            doorManager?.Disable();
        }
    }
}
