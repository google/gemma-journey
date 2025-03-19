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

/**
 * PlayerInteractable.cs
 * 
 * Manages player character interactions with interactable objects in the game environment.
 * 
 * This component handles detecting nearby interactable objects within a configurable
 * radius and angle, determining the best candidate for interaction based on distance
 * and angle weights, and triggering the interaction when player input is detected.
 * 
 * Requires a SphereCollider component on the same GameObject.
 */
using GoogleDeepMind.GemmaSampleGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerInteractable : MonoBehaviour
    {
        [Header("Interaction Settings")]
        /** Maximum distance for detecting interactable objects */
        [SerializeField] private float interactionRadius = 2f;

        /** Weight applied to distance when scoring interaction candidates */
        [SerializeField] private float distanceWeight = 1.0f;

        /** Maximum angle (in degrees) from forward direction for interactions */
        [SerializeField, Range(0, 180)] private float interactionAngle = 90f;

        /** Weight applied to angle when scoring interaction candidates */
        [SerializeField] private float angleWeight = 1.0f;

        /** Layer mask for interactable objects */
        [SerializeField] private LayerMask interactableLayer;

        /** Sphere collider that defines the interaction detection area */
        private SphereCollider collider;

        /** List of all interactable objects currently within range */
        private List<InteractableObject> interactables = new();

        /** Reference to the game state machine to check if interaction is allowed */
        private StateManagement.GameStateMachine gameStateMachine;

        /**
         * Initializes the interactor with dependencies.
         * 
         * @param gameStateMachine The game state machine that controls when interactions are available
         */
        [Inject]
        public void Construct(
            StateManagement.GameStateMachine gameStateMachine
        )
        {
            this.gameStateMachine = gameStateMachine;
        }

        /**
         * Retrieves the sphere collider component on initialization.
         */
        private void Awake()
        {
            collider = GetComponent<SphereCollider>();
        }

        /**
         * Configures the sphere collider with the interaction radius.
         */
        private void Start()
        {
            collider.radius = interactionRadius;
            collider.isTrigger = true;
        }

        /**
         * Checks for interaction input when in the appropriate game state.
         */
        private void Update()
        {
            if (gameStateMachine.IsInState(gameStateMachine.StateWalkAround))
            {
                HandleInput();
            }
        }

        /**
         * Adds interactable objects to the tracking list when they enter the detection range.
         * 
         * @param other The collider that entered the trigger zone
         */
        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & interactableLayer) != 0)
            {
                InteractableObject interactableObject = other.GetComponent<InteractableObject>();
                if (interactableObject != null)
                {
                    interactables.Add(interactableObject);
                }
            }
        }

        /**
         * Removes interactable objects from the tracking list when they exit the detection range.
         * 
         * @param other The collider that exited the trigger zone
         */
        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & interactableLayer) != 0)
            {
                InteractableObject interactable = other.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    interactables.Remove(interactable);
                }
            }
        }

        /**
         * Processes player input for interaction, finding the best candidate and triggering interaction.
         */
        private void HandleInput()
        {
            InteractableObject interactable = CheckInteract();
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (interactable != null)
                {
                    interactable.Interact(gameObject);
                }
            }
        }

        /**
         * Determines the best interactable object for interaction based on distance and angle.
         * Uses a weighted scoring system where lower scores are better.
         * 
         * @return The best interactable object, or null if none are available
         */
        private InteractableObject CheckInteract()
        {
            InteractableObject interactableObject = null;
            float minInteractionScore = float.MaxValue;
            List<InteractableObject> nullInteractable = new();
            foreach (var interactable in interactables)
            {
                if (interactable == null)
                {
                    nullInteractable.Add(interactable);
                    continue;
                }
                float distance = Vector3.Distance(interactable.transform.position, transform.position);
                float angle = Vector3.Angle(transform.forward, (interactable.transform.position - transform.position).normalized);
                // Ignore interactable that is outside of the interaction space for both
                if (distance >= interactionRadius || angle >= interactionAngle || !interactable.CanInteract(transform.position))
                {
                    continue;
                }
                float interactionScore = (distance * distanceWeight / interactionRadius) + (angle * angleWeight / interactionAngle);
                if (interactionScore < minInteractionScore)
                {
                    interactableObject = interactable;
                    minInteractionScore = interactionScore;
                }
            }

            if (nullInteractable.Count > 0)
            {
                foreach (var interactable in nullInteractable)
                {
                    interactables.Remove(interactable);
                }
            }
            return interactableObject;
        }

        /**
         * Draws debug visualization gizmos for the interaction radius and angle.
         * Only visible in the editor when the object is selected.
         */
        private void OnDrawGizmosSelected()
        {
            // Draw interaction radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);

            // Draw interaction arc
            Gizmos.color = Color.blue;
            Vector3 forward = transform.forward * interactionRadius;
            float halfAngle = interactionAngle * 0.5f * Mathf.Deg2Rad;
            Vector3 right = Quaternion.Euler(0, interactionAngle * 0.5f, 0) * forward;
            Vector3 left = Quaternion.Euler(0, -interactionAngle * 0.5f, 0) * forward;
            Gizmos.DrawLine(transform.position, transform.position + right);
            Gizmos.DrawLine(transform.position, transform.position + left);
        }
    }
}
