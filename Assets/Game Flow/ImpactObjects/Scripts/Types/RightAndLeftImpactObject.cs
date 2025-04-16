using System;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    internal class RightAndLeftImpactObject : ImpactObjectDecorator
    {
        public RightAndLeftImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats) : base(inner, mono, stats)
        {
        }

        public void Impact(Vector3 direction)
        {
            base.Impact(direction);
            Debug.Log("Moving Forward and Backward");
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            Debug.Log("Drawing Forward and Backward Gizmos");
        }
    }
}