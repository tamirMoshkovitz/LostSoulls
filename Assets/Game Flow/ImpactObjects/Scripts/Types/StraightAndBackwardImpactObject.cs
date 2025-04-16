using System;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class StraightAndBackwardImpactObject : ImpactObjectDecorator
    {
        public StraightAndBackwardImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats) : base(inner, mono, stats)
        {
        }

        public override void Impact(Vector3 direction)
        {
            base.Impact(direction);
            Debug.Log("Moving Right and Left");
        }
        
        public override void DrawGizmos()
        {
            base.DrawGizmos();
            Debug.Log("Drawing Right And Left Gizmos");
        }
    }
}