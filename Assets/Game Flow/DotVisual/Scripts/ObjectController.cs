using System;
using Core.Managers;
using Game_Flow.DotVisual.Scripts.States;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game_Flow.DotVisual.Scripts
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class ObjectController : MonoSingleton<ObjectController>
    {
        [SerializeField] private GameObject dotPrefab;
        
        private IObjeckLockingState _currentState;
        private Transform _origin;
        private InputSystem_Actions _inputSystemActions;
        private Vector2 _input;
        private MonoImpactObject _target;
        private bool _isLocked;
        public bool IsLocked { get => _isLocked; private set => _isLocked = value;}

        private void Awake()
        {
            if (dotPrefab != null)
            {
                dotPrefab = Instantiate(dotPrefab);
            }
            _currentState = new FPState();
            _origin = GetComponent<UnityEngine.Camera>().transform;
            _currentState.EnterState(_origin, dotPrefab);
            _inputSystemActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputSystemActions.Enable();
            _inputSystemActions.Player.Enable();
            _inputSystemActions.Player.Lock.performed += OnLock;
            _inputSystemActions.Player.Lock.canceled += OnUnlock;
            _inputSystemActions.Player.Move.performed += OnMovePerformed;
            _inputSystemActions.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            _inputSystemActions.Disable();
            _inputSystemActions.Player.Lock.performed -= OnLock;
            _inputSystemActions.Player.Lock.canceled -= OnUnlock;
            _inputSystemActions.Player.Enable();
            _inputSystemActions.Player.Move.performed += OnMovePerformed;
            _inputSystemActions.Player.Move.canceled += OnMoveCanceled;
        }

        private void Update()
        {
            _currentState?.Update();
            _currentState?.GetTarget(out _target);
            if (IsLocked)
            {
                Vector3 targetMovement = _currentState.CalculateMovement(_input);
                _target?.UpdateObject(targetMovement);
                Debug.Log(_input);
            }
        }
        
        public void ChangeState(IObjeckLockingState newState)
        {
            if (_currentState != null && _currentState.GetType() != newState.GetType())
            {
                _currentState.ExitState();
                _currentState = newState;
                _currentState.EnterState(_origin, dotPrefab);
            }
        }

        private void OnLock(InputAction.CallbackContext context)
        {
            if(_target != null) _target.Activate();
            IsLocked = true;
        }
        
        private void OnUnlock(InputAction.CallbackContext context)
        {
            if(_target != null) _target.DeActivate();
            IsLocked = false;
        }
        
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _input = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _input = Vector2.zero;
        }

    }
}