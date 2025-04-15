using UnityEngine;

namespace PlayerMovement
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

        
        public Vector3 Velocity => _velocity;
        public bool IsGrounded => _isGrounded;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _inputActions = new InputSystem_Actions();
        }

        void OnEnable()
        {
            _inputActions.Player.Enable();
            _inputActions.Player.Move.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Move.canceled += ctx => _movementInput = Vector2.zero;
            _inputActions.Player.Jump.performed += ctx => Jump();
        }

        void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        void Update()
        {
            HandleMovement();
        }

        void HandleMovement()
        {
            // Ground check
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (_isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            // Move input
            Vector3 move = transform.right * _movementInput.x + transform.forward * _movementInput.y;
            _controller.Move(move * moveSpeed * Time.deltaTime);

            // Gravity
            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        void Jump()
        {
            if (_isGrounded)
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}