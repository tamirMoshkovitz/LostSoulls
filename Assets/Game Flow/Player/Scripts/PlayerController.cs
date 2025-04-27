using Core.Managers;
using Game_Flow.DotVisual.Scripts;
using Game_Flow.DotVisual.Scripts.States;
using Game_Flow.ImpactObjects.Scripts.Types;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game_Flow.Player.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerController : MonoSingleton<PlayerController>
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private AudioClip whiteNoise;
        [SerializeField] private AudioClip concreteFloorSound;
        [SerializeField] private AudioClip woodFloorSound;
        [SerializeField] private AudioClip woodStairsSound;
        [SerializeField] private AudioSource stepsAudioSource;
        [SerializeField] private AudioSource BGAudioSource;

        private const string FirstFloorTag = "First Floor";
        private const string SecondFloorTag = "Second Floor";
        private const string StairsTag = "Stairs";
        
        private CharacterController _controller;
        private InputSystem_Actions _inputActions;
        private PlayerAudio _playerAudio;
        private Vector2 _movementInput;
        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _isMovementLocked;
        private string _currentFloor = FirstFloorTag;
        public bool IsMovementLocked {get => _isMovementLocked; set => _isMovementLocked = value;}

        
        public Vector3 Velocity => _velocity;
        public bool IsGrounded => _isGrounded;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
            _inputActions.Player.Enable();
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
            _inputActions.Player.Open.performed += OnOpenPerformed;
            EventManager.OnLockStateChanged += HandleLockStateChanged;
            _playerAudio = new PlayerAudio(stepsAudioSource, BGAudioSource, whiteNoise, concreteFloorSound, woodFloorSound, woodStairsSound);
            _playerAudio.PlayWhiteNoise();

        }

        void OnDisable()
        {
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Jump.performed -= OnJumpPerformed;
            _inputActions.Player.Open.performed -= OnOpenPerformed;
            _inputActions.Player.Disable();
            EventManager.OnLockStateChanged -= HandleLockStateChanged;
            _playerAudio = null;
        }

        void Update()
        {
            HandleMovement();
        }
        
        private void HandleLockStateChanged(IObjeckLockingState state)
        {
            _isMovementLocked = state is TopDownState;
        }

        void HandleMovement()
        {
            if (_isMovementLocked || ObjectController.Instance.IsLocked) {return;}
            // Ground check
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            
            if (Physics.Raycast(transform.position, Vector3.down, out var hit, Mathf.Infinity))
            {
                if (!hit.collider.CompareTag(_currentFloor))
                {
                    _currentFloor = hit.collider.gameObject.tag;
                }
            }

            if (_isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            // Move input
            Vector3 move = transform.right * _movementInput.x + transform.forward * _movementInput.y;
            _controller.Move(move * moveSpeed * Time.deltaTime);
            if (move.magnitude > 0.1f)
            {
                WalkingSurface surface = WalkingSurface.ConcreteFloor;
                switch (_currentFloor)
                {
                    case FirstFloorTag:
                        surface = WalkingSurface.ConcreteFloor;
                        break;
                    case StairsTag:
                        surface = WalkingSurface.WoodenStairs;
                        break;
                    case SecondFloorTag:
                        surface = WalkingSurface.WoodenFloor;
                        break;
                }
                _playerAudio.PlayFootstepSound(surface);
            }

            else
            {
                _playerAudio.StopFootstepSound();
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
        
        private void OnOpenPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Pressed Open Button");
            if (_isMovementLocked) return;
            Debug.DrawRay(gameObject.GetComponentInChildren<CinemachineCamera>().transform.position, gameObject.GetComponentInChildren<CinemachineCamera>().transform.forward, Color.magenta, 2f);
            Ray ray = new Ray(gameObject.GetComponentInChildren<CinemachineCamera>().transform.position, gameObject.GetComponentInChildren<CinemachineCamera>().transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 2f, LayerMask.GetMask("AnimationObject")))
            {
                Debug.Log("Ray hit something!");
                Debug.Log(hitInfo.collider.gameObject.name);
                var openable = hitInfo.collider.GetComponentInChildren<OpenCloseImpactObject>();
                if (openable != null)
                {
                    if (openable.IsOpen)
                    {
                        Debug.Log("Closing");
                        openable.CloseImpactObject();
                    }
                    else
                    {
                        Debug.Log("Opening");
                        openable.OpenImpactObject();
                    }
                }
            }
        }
        
    }
}