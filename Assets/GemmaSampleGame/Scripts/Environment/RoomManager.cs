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
            portalManager.PositionPortalAtCurrentRoom();
        }

        public void DisableRoom()
        {
            doorManager?.Disable();
        }
    }
}
