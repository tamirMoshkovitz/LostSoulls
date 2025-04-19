using Core.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game_Flow.PlayerMovement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundDistance = 0.4f;

        private CharacterController _controller;
        private InputSystem_Actions _inputActions;
        private Vector2 _movementInput;
        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _isMovementLocked;

        
        public Vector3 Velocity => _velocity;
        public bool IsGrounded => _isGrounded;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _inputActions = new InputSystem_Actions();
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"), 
                LayerMask.NameToLayer("BorderImpactObject"), 
                true
            );
        }

        void OnEnable()
        {
            _inputActions.Player.Enable();
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            _inputActions.Player.Jump.performed += OnJumpPerformed;
            EventManager.OnLockStateChanged += HandleLockStateChanged;
        }

        void OnDisable()
        {
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Jump.performed -= OnJumpPerformed;
            _inputActions.Player.Disable();
            EventManager.OnLockStateChanged -= HandleLockStateChanged;
        }

        void Update()
        {
            HandleMovement();
        }
        
        private void HandleLockStateChanged(bool isLocked)
        {
            _isMovementLocked = isLocked;
        }

        void HandleMovement()
        {
            // Ground check
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //TODO fix this

            if (_isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            // Move input
            if (!_isMovementLocked)
            {
                Vector3 move = transform.right * _movementInput.x + transform.forward * _movementInput.y;
                _controller.Move(move * moveSpeed * Time.deltaTime);
            }
            // Gravity
            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (_isGrounded)
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _movementInput = Vector2.zero;
        }
    }
}