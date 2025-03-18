using UnityEngine;
using System.Collections.Generic;
using VContainer;
using GoogleDeepMind.GemmaSampleGame.StateManagement;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class PortalDoorTrigger : MonoBehaviour
    {

        private PortalDoor portalDoor;
        private IRoomWallsRegistry registry;
        private LevelManager levelManager;
        private StateInputManager stateInputManager;

        [Inject]
        public void Construct(IRoomWallsRegistry registry, LevelManager levelManager, StateInputManager stateInputManager)
        {
            this.registry = registry;
            this.levelManager = levelManager;
            this.portalDoor = transform.parent.GetComponent<PortalDoor>();
            this.stateInputManager = stateInputManager;
        }

        /// <summary>
        /// Called when another collider enters the trigger volume
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // Check if the entering object is on the player layer
            if (portalDoor.isActive && ((1 << other.gameObject.layer) & portalDoor.playerLayer) != 0)
            {
                var allRoomWalls = registry.GetAllWalls();
                foreach (RoomWalls roomWall in allRoomWalls)
                {
                    roomWall.gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// Called when another collider stays within the trigger volume
        /// </summary>
        private void OnTriggerStay(Collider other)
        {
            // This can be used for continuous effects while the player is in the portal
            if (portalDoor.isActive && ((1 << other.gameObject.layer) & portalDoor.playerLayer) != 0)
            {
                // Optional: Add any continuous effects here
            }
        }

        /// <summary>
        /// Called when another collider exits the trigger volume
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            // Check if the exiting object is on the player layer
            if (((1 << other.gameObject.layer) & portalDoor.playerLayer) != 0)
            {
                // Calculate the direction from the portal to the player
                Vector3 portalForward = transform.forward;
                Vector3 playerDirection = other.transform.position - transform.position;

                // Use dot product to determine if player exited forward (new room) or backward (old room)
                float dotProduct = Vector3.Dot(portalForward, playerDirection.normalized);

                // Tell the GameStatemachine that player has enter a door
                if (dotProduct > 0)
                {
                    Debug.Log("Player exited portal into old room");
                }
                else
                {
                    Debug.Log("Player exited portal into new room");
                    stateInputManager.AddInput(new InputDoor(InputDoor.ActionType.Enter));
                }
                // Reactivate the walls
                var allRoomWalls = registry.GetAllWalls();
                foreach (RoomWalls roomWall in allRoomWalls)
                {
                    roomWall.gameObject.SetActive(true);
                }
            }
        }
    }
}