using System;
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
        private bool _isLocked = false;
        private InputSystem_Actions _inputActions;

        private void OnEnable()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();
            _inputActions.Player.Lock.started += OnLock;
            _inputActions.Player.Lock.canceled += OnLock;
        }
        
        private void OnDisable()
        {
            _inputActions.Player.Lock.started -= OnLock;
            _inputActions.Player.Lock.canceled -= OnLock;
            _inputActions.Player.Disable();
        }

        public void OnLock(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (targetingController.CurrentTarget != null && !_isLocked)
                {
                    lockedTarget = targetingController.CurrentTarget;
                    _isLocked = true;
                    lockedTarget.Activate(Vector3.left);
                    Debug.Log("Locked on to target: " + lockedTarget.name);
                }
                else
                {
                    Debug.Log("No target to lock on to.");
                }
            }
            else if (context.canceled)
            {
                lockedTarget = null;
                _isLocked = false;
                Debug.Log("Unlocked from target.");
            }
        }
    }
}