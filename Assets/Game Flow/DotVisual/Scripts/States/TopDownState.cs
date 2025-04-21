using Core.Managers;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game_Flow.DotVisual.Scripts.States
{
    public class TopDownState : IObjeckLockingState
    {
        private float speed = 2f;
        private Vector3 _position;
        private GameObject _dot;
        private Renderer _dotRenderer;
        private InputSystem_Actions _inputActions;
        private Vector2 _input;
        private MonoImpactObject _target;

        
        public void EnterState(Transform origin, GameObject dotInstance)
        {
            EventManager.LockStateChanged(this);
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
            _inputActions.Player.Enable();
            _position = origin.position;
            _dot = dotInstance;
            _dotRenderer = _dot.GetComponent<Renderer>();
            _inputActions.Player.Look.performed += OnMovePerformed;
            _inputActions.Player.Look.canceled += OnMoveCanceled;
            
            Debug.Log("Entered top state");
        }
        public void ExitState()
        {
            _inputActions.Player.Look.performed -= OnMovePerformed;
            _inputActions.Player.Look.canceled -= OnMoveCanceled;            
            _inputActions.Player.Disable();
            _inputActions.Disable();
        }

        public void Update()
        {
            if (ObjectController.Instance.IsLocked)
            {
                LockedUpdate();
                return;
            }
            UnLockedUpdate();
        }
        
        private void LockedUpdate()
        {
            _dot.transform.parent = _target?.transform;
        }

        private void UnLockedUpdate()
        {
            _position += new Vector3(-_input.x, 0, -_input.y) * speed * Time.deltaTime;
            if (Physics.Raycast(_position, Vector3.down, out var hit))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ImpactObject"))
                {
                    _dotRenderer.material.color = Color.green;
                    _target = hit.transform.gameObject.GetComponent<MonoImpactObject>();
                }
                else
                {
                    _dotRenderer.material.color = Color.red;
                    _target = null;
                }
                _dot.transform.position = hit.point;
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _input = _inputActions.Player.Look.ReadValue<Vector2>();
        }
        
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _input = Vector2.zero;
        }
        
        public MonoImpactObject GetTarget(out MonoImpactObject target)
        {
            return target = _target;
        }

        public Vector3 CalculateMovement(Vector2 input)
        {
            return new Vector3(-input.x, 0, -input.y) * speed * Time.deltaTime;
        }
    }
}