using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class LeftImpactObject : ImpactObjectDecorator
    {
        private readonly BoxCollider _boxCollider;
        private bool _activated = false;

        public LeftImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats)
            : base(inner, mono, stats)
        {
            _boxCollider = Mono.GetComponent<BoxCollider>();
        }

        public override void UpdateImpact(Vector3 direction)
        {
            base.UpdateImpact(direction);

            _activated = direction == Vector3.left;
            if (!_activated) return;

            Vector3 halfExtents = new Vector3(0f, _boxCollider.bounds.extents.y, _boxCollider.bounds.extents.z);
            float castDistance = _boxCollider.bounds.extents.x + Stats.bufferForRaycast;
            Vector3 origin = _boxCollider.bounds.center + Vector3.left * (halfExtents.x + 0.001f);

            bool blocked = Physics.BoxCast(
                origin,
                halfExtents,
                Vector3.left,
                Quaternion.identity,
                castDistance,
                Stats.impactObjectLayerMask | Stats.objectBorderLayerMask
            );

            if (!blocked)
            {
                Mono.transform.Translate(Vector3.left * Stats.speed * Time.deltaTime, Space.World);
            }
            else
            {
                Mono.IsBlocked = true;
                Debug.Log("Blocked (Left)");
            }
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if (!_activated) return;

            Vector3 halfExtents = new Vector3(0f, _boxCollider.bounds.extents.y, _boxCollider.bounds.extents.z);
            float castDistance = _boxCollider.bounds.extents.x + Stats.bufferForRaycast;
            Vector3 boxSize = halfExtents * 2f;
            Vector3 origin = _boxCollider.bounds.center + Vector3.left * castDistance;

            Gizmos.color = Color.blue;
            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
