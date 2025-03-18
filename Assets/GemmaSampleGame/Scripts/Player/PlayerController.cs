/**
 * PlayerController.cs
 * 
 * This class handles the player character's movement and animation in the game.
 * It processes input from the player, translates it into movement based on the camera orientation,
 * and updates the character's animation state accordingly.
 * 
 * The controller supports keyboard input only, with different movement speeds for walking and running.
 * Character rotation is smoothly interpolated to face the movement direction.
 */
using UnityEngine;
using VContainer;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float rotationSpeed = 720f;
        [SerializeField] private float movementThreshold = 0.1f;

        // Component references
        private Camera mainCamera;
        private StateManagement.GameStateMachine gameStateMachine;
        private UI.ControlOverlayUserInterface controlOverlayUserInterface;

        // Animation parameters
        private readonly int moveSpeedHash = Animator.StringToHash("Speed");
        private readonly int isRunningHash = Animator.StringToHash("IsRunning");

        // Movement variables
        private Vector3 movementDirection;
        private float currentSpeed;
        private bool isRunning;

        /// <summary>
        /// Dependency injection method for required game systems.
        /// Receives references to the game state machine and control overlay UI.
        /// </summary>
        /// <param name="gameStateMachine">Reference to the game state machine for state-dependent behavior</param>
        /// <param name="controlOverlayUserInterface">Reference to the control overlay UI for input visualization</param>
        [Inject]
        public void Construct(
            StateManagement.GameStateMachine gameStateMachine,
            UI.ControlOverlayUserInterface controlOverlayUserInterface
        )
        {
            this.gameStateMachine = gameStateMachine;
            this.controlOverlayUserInterface = controlOverlayUserInterface;
        }

        /// <summary>
        /// Initializes the character controller and sets up required components.
        /// Gets a reference to the main camera and configures the rigidbody physics settings.
        /// </summary>
        private void Start()
        {
            mainCamera = Camera.main;

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        /// <summary>
        /// Called every frame to process input and update animations.
        /// Handles player input processing and updates the character's animation state.
        /// </summary>
        private void Update()
        {
            HandleInput();
            UpdateAnimation();
        }

        /// <summary>
        /// Called at fixed intervals for physics-based movement.
        /// Applies the calculated movement to the character's rigidbody.
        /// </summary>
        private void FixedUpdate()
        {
            HandleMovement();
        }

        /// <summary>
        /// Processes player input and converts it to movement direction.
        /// Takes into account the camera orientation to make movement relative to the camera view.
        /// Supports both gamepad (left stick/dpad) and keyboard (WASD) input.
        /// </summary>
        private void HandleInput()
        {
            // Early out if not in WalkingAround state
            if (gameStateMachine == null || !gameStateMachine.IsInState(gameStateMachine.StateWalkAround))
            {
                movementDirection = Vector3.zero;
                isRunning = false;
                return;
            }

            Vector3 moveInput = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) { moveInput.y += 1; }
            if (Input.GetKey(KeyCode.S)) { moveInput.y -= 1; }
            if (Input.GetKey(KeyCode.D)) { moveInput.x += 1; }
            if (Input.GetKey(KeyCode.A)) { moveInput.x -= 1; }
            moveInput = moveInput.normalized;
            controlOverlayUserInterface.ToggleControlButton(moveInput);

            // Check for run input
            isRunning = Input.GetKey(KeyCode.LeftShift);

            // Convert input to world space direction based on camera
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            // Project vectors onto XZ plane
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Calculate movement direction
            movementDirection = (forward * moveInput.y + right * moveInput.x).normalized;
        }

        /// <summary>
        /// Handles the actual movement of the character based on the calculated movement direction.
        /// Rotates the character to face the movement direction and applies the appropriate speed.
        /// Uses Rigidbody.MovePosition for physics-based movement.
        /// </summary>
        private void HandleMovement()
        {
            if (movementDirection.magnitude > movementThreshold)
            {
                // Rotate towards movement direction
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                // Move in the input direction with appropriate speed
                float currentMoveSpeed = isRunning ? runSpeed : walkSpeed;
                Vector3 movement = movementDirection * (currentMoveSpeed * Time.fixedDeltaTime);
                _rigidbody.MovePosition(_rigidbody.position + movement);

                // Make the transition to moving more immediate
                currentSpeed = 1f;
            }
            else
            {
                // Make the transition to idle more immediate
                currentSpeed = 0f;
            }
        }

        /// <summary>
        /// Updates the animator parameters based on the current movement state.
        /// Sets the Speed parameter for walk/idle animation blending and IsRunning for run animations.
        /// </summary>
        private void UpdateAnimation()
        {
            _animator.SetFloat(moveSpeedHash, currentSpeed);
            _animator.SetBool(isRunningHash, isRunning && movementDirection.magnitude > movementThreshold);
        }

        /// <summary>
        /// Sets the character's position directly, bypassing physics.
        /// Useful for teleportation, initial positioning, or level transitions.
        /// </summary>
        /// <param name="position">The new world position for the character</param>
        public void SetPosition(Vector3 position)
        {
            _rigidbody.MovePosition(position);
        }
    }
}