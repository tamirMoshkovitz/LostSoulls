using System.Collections.Generic;
using Core.Managers;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.DotVisual.Scripts.States
{
    public class TopDownState : IObjeckLockingState
    {
        private const float Speed = 2f;
        private Vector3 _position;
        private GameObject _dot;
        private Renderer _dotRenderer;
        private InputSystem_Actions _inputActions;
        private Vector2 _input;
        private MonoImpactObject _target;
        private InputAction _inputReader;
        
        private float _moveCooldown = 0.3f;
        private float _lastMoveTime = -Mathf.Infinity;
        private LayerMask _impactLayerMask = LayerMask.GetMask("ImpactObject");
        private Grid _grid;
        private (int row, int col) _currentCell;
        private bool _isMoving = false;
        
        
        public void EnterState(Transform origin, GameObject dotInstance)
        {
            EventManager.LockStateChanged(this);
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
            _inputActions.Player.Enable();
            _inputReader = _inputActions.Player.Move;
            _position = origin.position;
            _dot = dotInstance;
            _dotRenderer = _dot.GetComponent<Renderer>();
            _inputReader.performed += OnMovePerformed;
            _inputReader.canceled += OnMoveCanceled;
            _dotRenderer.enabled = true;
            Debug.Log("Entered top state");
            
            _grid = Object.FindFirstObjectByType<Grid>();
            _currentCell = _grid.WorldToCell(_dot.transform.position);
        }
        public void ExitState()
        {
            _dotRenderer.enabled = false;
            _inputReader.performed -= OnMovePerformed;
            _inputReader.canceled -= OnMoveCanceled;            
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
            if (_isMoving || _input == Vector2.zero || Time.time - _lastMoveTime < _moveCooldown)
                return;
            
            Debug.Log("input: " + _input);
            Vector3 inputDir = new Vector3(_input.x, 0, _input.y).normalized;
            _lastMoveTime = Time.time;
            _isMoving = true;
            _dotRenderer.material.color = Color.red; // Default red unless target found

            CoroutineRunner.Instance.StartCoroutine(MoveUntilObjectFound(inputDir));
            /**_position += new Vector3(-_input.x, 0, -_input.y) * Speed * Time.deltaTime;
            if (Physics.Raycast(_position, Vector3.down, out var hit))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ImpactObject"))
                {
                    _dotRenderer.material.color = Color.green;
                    _target = hit.transform.gameObject.GetComponent<MonoImpactObject>();
                    if (_target != null && _target.IsMoveable)
                    {
                        _target.HighlightObject();
                    }
                }
                else
                {
                    if (_target != null)
                    {
                        _target.UnhighlightObject();
                    }
                    _dotRenderer.material.color = Color.red;
                    _target = null;
                }
                _dot.transform.position = hit.point;
            }**/
            
        }
        
        private System.Collections.IEnumerator MoveUntilObjectFound(Vector3 direction)
        {
            Debug.Log("Started coroutine to move dot...");
            int dRow = 0, dCol = 0;
            if (Vector3.Dot(direction, Vector3.forward) > 0.5f) dRow = +1;
            else if (Vector3.Dot(direction, Vector3.back) > 0.5f) dRow = -1;
            else if (Vector3.Dot(direction, Vector3.right) > 0.5f) dCol = +1;
            else if (Vector3.Dot(direction, Vector3.left) > 0.5f) dCol = -1;
            else { _isMoving = false; yield break; }

            while (true)
            {
                var nextCell = (row: _currentCell.row + dRow, col: _currentCell.col + dCol);
                Debug.Log($"Current cell: {_currentCell}, Next cell: {nextCell}");
                // Stop if out of bounds
                if (nextCell.row < 0 || nextCell.row >= _grid.Rows || nextCell.col < 0 || nextCell.col >= _grid.Cols)
                    break;

                List<(int row, int col)> check = new() { nextCell };

                if (_grid.IsCellsOccupied(check))
                {
                    _currentCell = nextCell;
                    Vector3 newPos = _grid.GetWorldCenter(_currentCell);
                    Debug.Log($"Moving dot to: {newPos}");
                    _dot.transform.position = newPos;
                    _target = _grid.GetOccupant(nextCell.row, nextCell.col);
                    _dotRenderer.material.color = Color.green;
                    _target?.HighlightObject();
                    Debug.Log($"Found target: {_target}");
                    break;
                }

                _currentCell = nextCell;
                _dot.transform.position = _grid.GetWorldCenter(_currentCell);
                yield return new WaitForSeconds(0.05f);
            }

            _isMoving = false;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _input = _inputReader.ReadValue<Vector2>();
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
            return new Vector3(-input.x, 0, -input.y) * Speed * Time.deltaTime;
        }
    }
}