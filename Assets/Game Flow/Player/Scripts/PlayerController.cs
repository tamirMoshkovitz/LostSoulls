using Core.Input_System;
using Core.Managers;
using Game_Flow.CollectableObjects;
using Game_Flow.DotVisual.Scripts;
using Game_Flow.DotVisual.Scripts.States;
using Game_Flow.ImpactObjects.Scripts.Types;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using Game_Flow.UI;
using OpeningScene;
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
        [SerializeField] private ItemsUpdater itemsUpdater;

        [Header("Rumble")] [SerializeField] private float rumbleDuration = 0.5f;
        [SerializeField] private float lowFrequency = 0.1f;
        [SerializeField] private float highFrequency = 0.5f;

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
        private bool _collectedDoll = false;
        private bool _hasStarted = false;
        private bool _isTopDown = false;

        public bool IsMovementLocked
        {
            get => _isMovementLocked;
            set => _isMovementLocked = value;
        }

        public Vector3 Velocity => _velocity;
        public bool IsGrounded => _isGrounded;

        public InputSystem_Actions InputActions => _inputActions;

        private HandleInteractableObjects _interactableObjectsHandler;
        private GameObject _lastInteractableHit;


        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
            _inputActions.Player.Disable();
            _inputActions.OpeningScene.Enable();
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("BorderImpactObject"),
                true
            );
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("ImpactObject"),
                true
            );
            IsMovementLocked = true;
            _interactableObjectsHandler = gameObject.GetComponent<HandleInteractableObjects>();
        }

        void OnEnable()
        {
            //_inputActions.Player.Enable();
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            _inputActions.Player.Jump.performed += OnJumpPerformed;
            _inputActions.Player.Open.performed += OnOpenPerformed;
            _inputActions.OpeningScene.Open.performed += OnOpenPerformed;
            EventManager.OnLockStateChanged += HandleLockStateChanged;
            _playerAudio = new PlayerAudio(stepsAudioSource, BGAudioSource, whiteNoise, concreteFloorSound,
                woodFloorSound, woodStairsSound);
            _playerAudio.PlayWhiteNoise();
            EventManager.OnDollPlaced += OnTopDown;

        }

        void OnDisable()
        {
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Jump.performed -= OnJumpPerformed;
            _inputActions.Player.Open.performed -= OnOpenPerformed;
            _inputActions.OpeningScene.Open.performed -= OnOpenPerformed;
            _inputActions.Player.Disable();
            EventManager.OnLockStateChanged -= HandleLockStateChanged;
            _playerAudio = null;
            EventManager.OnDollPlaced -= OnTopDown;
        }

        void Update()
        {
            HandleMovement();
            if (!_isTopDown)
            {
                RumbleWhenObjectFound();
            }
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

        private void RumbleWhenObjectFound() //TODO: fix exception on destroy
        {
            Debug.DrawRay(gameObject.GetComponentInChildren<CinemachineCamera>().transform.position, gameObject.GetComponentInChildren<CinemachineCamera>().transform.forward, Color.red);
            Ray ray = new Ray(gameObject.GetComponentInChildren<CinemachineCamera>().transform.position, gameObject.GetComponentInChildren<CinemachineCamera>().transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 2f, LayerMask.GetMask("AnimationObject", "CollectableObject")))
            {
                if (_lastInteractableHit != null && _lastInteractableHit.Equals(hitInfo.collider.gameObject)) return;
                EventManager.StartRumble(rumbleDuration, lowFrequency, highFrequency);
                _lastInteractableHit = hitInfo.collider.gameObject;
            }
        }
        
        private void OnOpenPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Pressed Open Button");
            
            if (!_hasStarted)
            {
                _hasStarted = true;
                OpeningSceneController.Instance.OnStartPressed();
                _isMovementLocked = false;
            }
            
            if (_isMovementLocked) return;
            
            Debug.DrawRay(gameObject.GetComponentInChildren<CinemachineCamera>().transform.position, gameObject.GetComponentInChildren<CinemachineCamera>().transform.forward, Color.magenta, 2f);
            Ray ray = new Ray(gameObject.GetComponentInChildren<CinemachineCamera>().transform.position, gameObject.GetComponentInChildren<CinemachineCamera>().transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 2f, LayerMask.GetMask("AnimationObject")) && (_interactableObjectsHandler.InHighlightZone || _interactableObjectsHandler.InLetterZone))
            {
                Debug.Log("Ray hit something!");
                Debug.Log(hitInfo.collider.gameObject.name);
                var openable = hitInfo.collider.GetComponentInChildren<OpenCloseImpactObject>();
                if (openable != null)
                {
                    if (openable.IsLocked)
                    {
                        openable.PlayLockedAnimation();
                        Debug.Log("Object is locked");
                        return;
                    }
                    if (openable.IsOpen)
                    {
                        Debug.Log("Closing");
                        openable.CloseImpactObject();
                    }
                    else
                    {
                        Debug.Log("Opening");
                        itemsUpdater.ClearAll();
                        openable.OpenImpactObject();
                    }
                }
            }
            else if (Physics.Raycast(ray, out hitInfo, 2f, LayerMask.GetMask("AnimationObject")) && (_interactableObjectsHandler.InHighlightZone || _interactableObjectsHandler.InLetterZone))
            {
                Debug.Log("Ray hit something!");
                Debug.Log(hitInfo.collider.gameObject.name);
                var openable = hitInfo.collider.GetComponentInChildren<OpenCloseImpactObject>();
                if (openable != null)
                {
                    if (openable.IsLocked)
                    {
                        openable.PlayLockedAnimation();
                        Debug.Log("Object is locked");
                        return;
                    }
                    if (openable.IsOpen)
                    {
                        
                        Debug.Log("Closing");
                        openable.CloseImpactObject();
                    }
                    else
                    {
                        Debug.Log("Opening");
                        itemsUpdater.ClearAll();
                        openable.OpenImpactObject();
                    }
                }
            }
            else if (Physics.Raycast(ray, out hitInfo, 2f, LayerMask.GetMask("CollectableObject")))
            {
                Debug.Log("Ray hit something!");
                Debug.Log(hitInfo.collider.gameObject.name);
                var switchObject = hitInfo.collider.gameObject.GetComponent<SwitchObject>();
                if (switchObject != null)
                {
                    switchObject.ControlLights();
                }
                var collectable = hitInfo.collider.GetComponentInChildren<CollectableKeyObject>();
                var tag = hitInfo.collider.gameObject.tag;
                if (collectable != null)
                {
                    collectable.OnCollect();
                }
                var collectableDoll = hitInfo.collider.GetComponentInChildren<CollectableDollObject>();
                if (collectableDoll != null)
                {
                    collectableDoll.OnCollect();
                    _collectedDoll = true;
                }
                var collectableManDoll = hitInfo.collider.GetComponentInChildren<CollectableManDollObject>();
                if (collectableManDoll != null && _collectedDoll)
                {
                    collectableManDoll.OnCollect(OnDollPlaced);
                    _collectedDoll = true;
                }
                
            }
            else
            {
                Debug.Log("Ray did not hit anything.");
            }
        }

        private void OnDollPlaced()
        {
            EventManager.DollPlaced();
            _interactableObjectsHandler.CloseAllOpenedObjects();
        }

        private void OnTopDown()
        {
            _isTopDown = true;
        }
    }
}