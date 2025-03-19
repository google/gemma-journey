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
using UnityEngine.Events;

namespace GoogleDeepMind.GemmaSampleGame
{
    [RequireComponent(typeof(SphereCollider))]
    public class InteractableObject : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 2f;
        [SerializeField] private LayerMask playerLayer;

        [SerializeField] private UnityEvent<GameObject> onInteract;

        private SphereCollider triggerCollider;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            triggerCollider = GetComponent<SphereCollider>();
            meshRenderer = GetComponent<MeshRenderer>();

            // Setup trigger collider
            triggerCollider.isTrigger = true;
            triggerCollider.radius = interactionRadius;
        }

        public bool CanInteract(Vector3 position)
        {
            return gameObject.activeInHierarchy && Vector3.Distance(position, transform.position) < interactionRadius;
        }

        public void Interact(GameObject gameObject)
        {
            onInteract?.Invoke(gameObject);
        }
    }
}