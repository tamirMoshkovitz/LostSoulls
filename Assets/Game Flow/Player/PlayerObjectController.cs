using System;
using Core.Managers;
using Game_Flow.DotVisual.Scripts;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game_Flow.PlayerMovement
{
    public class PlayerObjectController : MonoBehaviour
    {
        [SerializeField] private DotVisualController targetingController;
        
        private MonoImpactObject lockedTarget;
        private static bool _isLocked = false;
        private InputSystem_Actions _inputActions;
        private Vector2 _moveInput;
        

        private void OnEnable()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();
            _inputActions.Player.Lock.started += OnLock;
            _inputActions.Player.Lock.canceled += OnLock;
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
        }
        
        private void OnDisable()
        {
            _inputActions.Player.Lock.started -= OnLock;
            _inputActions.Player.Lock.canceled -= OnLock;
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Disable();
        }
        
        private void SetLockState(bool state)
        {
            if (_isLocked != state)
            {
                _isLocked = state;
                EventManager.LockStateChanged(_isLocked); // 🟢 Call the EventManager
            }
        }

        public void OnLock(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (targetingController.CurrentTarget != null && !_isLocked)
                {
                    lockedTarget = targetingController.CurrentTarget;
                    SetLockState(true);
                }
            }
            else if (context.canceled)
            {
                lockedTarget = null;
                SetLockState(false);
            }
        }
        
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        public void Update()
        {
            if (_isLocked && lockedTarget != null && _moveInput != Vector2.zero)
            {
                Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y);
                lockedTarget.Activate(moveDirection);
            }
        }
    }
}