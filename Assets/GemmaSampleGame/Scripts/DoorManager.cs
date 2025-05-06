/**
 * DoorManager.cs
 * 
 * This class handles the behavior of interactive doors in the game environment.
 * It manages door opening and closing animations, collision triggers, and player interactions.
 * 
 * The door can detect when a player enters its trigger area and can be opened or closed 
 * through direct method calls or state input events. When open, the door's physical collider
 * is disabled to allow passage.
 */
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class DoorManager : MonoBehaviour
    {
        /** Animator component controlling door animation */
        [SerializeField] private Animator animator;

        /** Layer mask for detecting player objects */
        [SerializeField] private LayerMask playerLayer;

        /** Physical collider that blocks player movement when door is closed */
        [SerializeField] private Collider rigidCollider;

        /** Trigger collider for detecting player entry */
        private BoxCollider boxCollider;

        /** Animation parameter hash for the IsOpen property */
        private int IsOpenHash = Animator.StringToHash("IsOpen");

        /** Reference to the state input manager for handling door-related input events */
        private StateManagement.StateInputManager _stateInputManager;

        /** Current open/closed state of the door */
        public bool IsOpen => _isOpen;
        private bool _isOpen;

        /**
         * Dependency injection method for required game systems.
         * Receives a reference to the state input manager for handling door-related input events.
         * 
         * @param stateInputManager Reference to the state input manager
         */
        [Inject]
        public void Construct(
            StateManagement.StateInputManager stateInputManager
            )
        {
            _stateInputManager = stateInputManager;
        }

        /**
         * Initializes the door manager and sets up required components.
         * Configures the box collider as a trigger for player detection.
         */
        private void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        /**
         * Called every frame to process door-related input events.
         * Handles door opening, closing, and entry actions based on received inputs.
         */
        private void Update()
        {
            if (_stateInputManager != null && _stateInputManager.HasInput<StateManagement.InputDoor>())
            {
                var input = _stateInputManager.GetLastInput<StateManagement.InputDoor>();
                switch (input.Type)
                {
                    case StateManagement.InputDoor.ActionType.Open:
                        Open();
                        break;
                    case StateManagement.InputDoor.ActionType.Close:
                        Close();
                        break;
                    case StateManagement.InputDoor.ActionType.Enter:
                        break;
                }
            }
        }

        /**
         * Opens the door by setting the animation state and disabling the physical collider.
         * Allows player characters to pass through the doorway.
         */
        public void Open()
        {
            animator.SetBool(IsOpenHash, true);
            rigidCollider.enabled = false;
            _isOpen = true;
        }

        /**
         * Closes the door by setting the animation state and enabling the physical collider.
         * Prevents player characters from passing through the doorway.
         */
        public void Close()
        {
            animator.SetBool(IsOpenHash, false);
            rigidCollider.enabled = true;
            _isOpen = false;
        }

        /**
         * Enables the door interaction system.
         * Activates both the component and the trigger collider.
         */
        public void Enable()
        {
            enabled = true;
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider>();
            }
            boxCollider.enabled = true;
        }

        /**
         * Disables the door interaction system.
         * Deactivates both the component and the trigger collider.
         */
        public void Disable()
        {
            enabled = false;
            boxCollider.enabled = false;
        }

        /**
         * Triggered when an object enters the door's trigger collider.
         * If the door is open and the entering object is on the player layer,
         * generates an entry input event.
         * 
         * @param other The collider that entered the trigger area
         */
        private void OnTriggerEnter(Collider other)
        {
            if (_isOpen && ((1 << other.gameObject.layer) & playerLayer) != 0)
            {
                _stateInputManager.AddInput(new StateManagement.InputDoor(StateManagement.InputDoor.ActionType.Enter));
            }
        }
    }
}
