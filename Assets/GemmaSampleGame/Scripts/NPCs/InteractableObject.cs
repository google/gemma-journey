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