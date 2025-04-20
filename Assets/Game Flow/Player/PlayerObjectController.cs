using System;
using Core.Managers;
using Game_Flow.Camera;
using Game_Flow.DotVisual.Scripts;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game_Flow.Player
{
    public class PlayerObjectController : MonoBehaviour
    {
        [SerializeField] private DotVisualController targetingController;
        [SerializeField] private CinemachineCamera firstPersonCamera;
        [SerializeField] private CinemachineCamera topDownCamera;
        
        private MonoImpactObject lockedTarget;
        private static bool _isLocked = false;
        private InputSystem_Actions _inputActions;
        private Vector2 _moveInput;
        private bool isInTopDownView = false;
        private Vector3 cachedRayOriginPosition;


        private void OnEnable()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();
            _inputActions.Player.Lock.started += OnLock;
            _inputActions.Player.Lock.canceled += OnLock;
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            EventManager.OnViewModeChanged += OnViewModeChanged;
        }
        
        private void OnDisable()
        {
            _inputActions.Player.Lock.started -= OnLock;
            _inputActions.Player.Lock.canceled -= OnLock;
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            EventManager.OnViewModeChanged -= OnViewModeChanged;
            _inputActions.Player.Disable();
        }
        
        private void SetLockState(bool state)
        {
            if (_isLocked != state)
            {
                _isLocked = state;
                // EventManager.LockStateChanged(_isLocked); // 🟢 Call the EventManager
                targetingController.IsLocked = _isLocked; // 🟢 Update the targeting controller
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
            // if (!isInTopDownView) return; //TODO: uncomment this line to disable movement in only top-down view

            if (_moveInput == Vector2.zero)
            {
                Debug.Log("No movement input detected.");
                return;
            }

            if (_isLocked && lockedTarget != null)
            {
                Vector3 camForward = firstPersonCamera.transform.forward;
                Vector3 camRight = firstPersonCamera.transform.right;
                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();
                Vector3 moveDirection = camForward * _moveInput.y + camRight * _moveInput.x;
                lockedTarget.Activate(moveDirection);
            }
            else
            {
                Vector3 moveDirection = new Vector3(_moveInput.x, _moveInput.y, 0f);
                targetingController.MoveRayOrigin(moveDirection);
            }
        }
        
        private void OnViewModeChanged(ViewMode mode)
        {
            if (mode == ViewMode.TopDown)
            {
                isInTopDownView = true;
                targetingController.SetTopDownMode(true);
            }
            else
            {
                isInTopDownView = false;
                targetingController.SetTopDownMode(false);
                targetingController.ResetRayOriginLocalPosition();
            }
        }
    }
} 