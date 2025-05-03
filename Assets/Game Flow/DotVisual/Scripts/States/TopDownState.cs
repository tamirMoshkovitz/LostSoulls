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
        // private Renderer _dotRenderer;
        private InputSystem_Actions _inputActions;
        private Vector2 _input;
        private MonoImpactObject _target;
        private MonoImpactObject _lastTarget;
        private InputAction _inputReader;
        
        private float _moveCooldown = 0.3f;
        private float _lastMoveTime = -Mathf.Infinity;
        private LayerMask _impactLayerMask = LayerMask.GetMask("ImpactObject");
        
        private bool _isMoving = false;
        private List<MonoImpactObject> _impactObjects;
        private ObjectController _cameraMono;

        public Vector3 DebugInputDir   { get;  set; }
        public MonoImpactObject DebugNextTarget { get; set; }
        public void EnterState(Transform origin, GameObject dotInstance, List<MonoImpactObject> impactObjects,
            ObjectController objectController)
        {
            EventManager.LockStateChanged(this);
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
            _inputActions.Player.Enable();
            _inputReader = _inputActions.Player.Move;
            _position = origin.position;
            _inputReader.performed += OnMovePerformed;
            _inputReader.canceled += OnMoveCanceled;
            Debug.Log("Entered top state");
            _impactObjects = impactObjects;
            _target = impactObjects[0];
            _target.HighlightObject();
            _cameraMono = objectController;
        }
        public void ExitState()
        {
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
            
        }

        private void UnLockedUpdate()
        {
            if (_input == Vector2.zero || Time.time - _lastMoveTime < _moveCooldown)
                return;
            // 0) get raw stick into 3D
            Vector3 raw3D = new Vector3(_input.x, 0f, _input.y);

            // 1) rotate by camera yaw
            float camYaw = _cameraMono.transform.eulerAngles.y;
            Quaternion yawRot = Quaternion.Euler(0f, camYaw, 0f);
            Vector3 camRelative = yawRot * raw3D;

            // 2) quantize on the XZ‐plane
            Vector2 cam2D    = new Vector2(camRelative.x, camRelative.z);
           
            var inputDir = QuantizeTo8Directions(cam2D);
            DebugInputDir  = inputDir;

            if (inputDir == Vector3.zero)
                return;

            _lastMoveTime = Time.time;
            var newTarget  = FindNearestInDirection(inputDir);
            
            DebugNextTarget = newTarget;
            var oldTarget = _target;

            if (newTarget != null && newTarget != _target)
            {
                // we found a new one – unhighlight the old, highlight the new
                oldTarget?.UnhighlightObject();
                _target = newTarget;
                if (_target.IsMoveable)
                    _target.HighlightObject();
            }
        }
        
        
        
        private MonoImpactObject FindNearestInDirection(Vector3 dir)
        {

            MonoImpactObject best       = null;
            float              bestAng   = float.MaxValue;
            float              bestDistSq= float.MaxValue;

            foreach (var obj in _impactObjects)
            {
                // 2) skip the one already highlighted
                if (obj == _target) continue;

                // 3) compute flat‐XZ vector from origin to candidate
                Vector3 toObj = obj.transform.position - _target.transform.position;
                toObj.y = 0;
                float distSq = toObj.sqrMagnitude;
                if (distSq < 0.0001f) continue;

                Vector3 toDir = toObj.normalized;
                float   angle = Vector3.Angle(dir, toDir);

                // only look in your forward hemisphere
                if (angle > 90f) continue;

                // pick the smallest angle (tie‐break by closer distance)
                if (angle < bestAng || (Mathf.Approximately(angle, bestAng) && distSq < bestDistSq))
                {
                    best       = obj;
                    bestAng    = angle;
                    bestDistSq = distSq;
                }
            }
            return best;
        }
        
        /// <summary>
        /// Takes a raw input Vector2 (x,y) in [–1…1] and returns
        /// one of the eight unit directions (E,NE,N,NW,W,SW,S,SE).
        /// </summary>
        private Vector3 QuantizeTo8Directions(Vector2 rawInput)
        {

            // 1) get 0–360° angle (where 0° = +X/East, 90° = +Y/North)
            float angle = Mathf.Atan2(rawInput.y, rawInput.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            // 2) snap to nearest multiple of 45°
            int sector = Mathf.RoundToInt(angle / 45f) % 8;
            float snappedAngle = sector * 45f * Mathf.Deg2Rad;

            // 3) rebuild a unit vector from that snapped angle
            Vector2 dir2D = new Vector2(Mathf.Cos(snappedAngle), Mathf.Sin(snappedAngle));
            return new Vector3(dir2D.x, 0f, dir2D.y);
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
            return new Vector3(-input.x, 0, -input.y) * (Speed * Time.deltaTime);
        }
    }
}