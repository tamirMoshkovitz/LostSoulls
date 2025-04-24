using System;
using System.Collections.Generic;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.Types;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts
{
    public class MonoImpactObject : MonoBehaviour
    {
        private IImpactObject _impactObject;
        [SerializeField] private List<ImpactObjectTypes> decoratorOrder;
        [SerializeField] private ImpactObjectStats stats;
        [SerializeField] private MultiImpactObjectLinker linker;
        [FormerlySerializedAs("gridVisualizer")] [SerializeField] private Grid grid;
        [SerializeField] private List<MonoImpactObject> nonCollidingObjects = new List<MonoImpactObject>();
        
        private bool _updated;
        private bool _activated;
        public bool IsSoul {get; private set;}
        public bool IsBlocked { get; set; } = false;
        public List<MonoImpactObject> NonCollidingObjects => nonCollidingObjects;
        void Start()
        {
            IsSoul = false;
            _impactObject = new BasicImpactObject(this,stats);
            
            foreach (var type in decoratorOrder)
            {
                if (type == ImpactObjectTypes.Soul) IsSoul = true;
                bool shouldSnapToGrid = grid != null && type is ImpactObjectTypes.OneBlockGrid
                    or ImpactObjectTypes.TwoBlockHorizontalGrid
                    or ImpactObjectTypes.TwoBlockVerticalGrid
                    or ImpactObjectTypes.ThreeBlockHorizontalGrid
                    or ImpactObjectTypes.ThreeBlockVerticalGrid
                    or ImpactObjectTypes.FourBlocksSquareGrid;
                
                
                _impactObject = ImpactObjectFactory.CreateImpactObject(type, _impactObject, this,stats,grid);
                if(shouldSnapToGrid) _impactObject.StopImpact();
            }
        }


        public void Activate()
        {
            if(_activated) return;
            _activated = true;
            _impactObject.StartImpact();
            if (linker != null)
                linker.ActivateSiblings();
        }
        
        public void UpdateObject(Vector3 direction)
        {
            if (_updated || direction.Equals(Vector3.zero)) return;
            _updated = true;
            Vector3 snapped = GetClosestCardinalDirection(direction);
            IsBlocked = false;
            _impactObject.UpdateImpact(snapped);
            if (linker != null)
            {
                linker.UpdateSiblings(snapped);
                linker.ConnectObjects();
            }
        }
        
        public void DeActivate()
        {
            if(!_activated) return;
            _activated = false;
            _impactObject.StopImpact();
            if (linker != null)
                linker.DeActivateSiblings();
        }

        public void Update()
        {
            _updated = false;
        }

        void OnDrawGizmos()
        {
            _impactObject?.DrawGizmos();
        }
        
        private Vector3 GetClosestCardinalDirection(Vector3 direction)
        {
            direction.y = 0; // Ignore vertical component
            direction.Normalize();

            Vector3[] cardinalDirections =
            {
                Vector3.forward,  // (0, 0, 1)
                Vector3.back,     // (0, 0, -1)
                Vector3.right,    // (1, 0, 0)
                Vector3.left      // (-1, 0, 0)
            };

            Vector3 best = Vector3.zero;
            float maxDot = -Mathf.Infinity;

            foreach (var dir in cardinalDirections)
            {
                float dot = Vector3.Dot(direction, dir);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    best = dir;
                }
            }

            return best;
        }
        
        public Vector3 GetBottomCenter()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer == null) return transform.position;

            Bounds bounds = renderer.bounds;
            Vector3 bottomCenter = bounds.center;
            bottomCenter.y = bounds.min.y;
            return bottomCenter;
        }

    }
}