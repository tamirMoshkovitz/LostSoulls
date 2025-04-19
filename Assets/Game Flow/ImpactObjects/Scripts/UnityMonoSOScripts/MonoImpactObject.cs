using System.Collections.Generic;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.Types;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts
{
    public class MonoImpactObject : MonoBehaviour
    {
        private IImpactObject _impactObject;
        [SerializeField] private List<ImpactObjectTypes> decoratorOrder;
        [SerializeField] private ImpactObjectStats stats;
        public bool IsSoul {get; private set;}
        void Start()
        {
            IsSoul = false;
            _impactObject = new BasicImpactObject(this,stats);
            
            foreach (var type in decoratorOrder)
            {
                if (type == ImpactObjectTypes.Soul) IsSoul = true; 
                _impactObject = ImpactObjectFactory.CreateImpactObject(type, _impactObject, this,stats);
            }
        }

        
        
        
        public void Activate(Vector3 direction)
        {
            Vector3 snapped = GetClosestCardinalDirection(direction);
            _impactObject.Impact(snapped);
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

    }
}