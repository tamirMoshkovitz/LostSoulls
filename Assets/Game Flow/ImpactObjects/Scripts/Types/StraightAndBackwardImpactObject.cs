using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class StraightAndBackwardImpactObject : ImpactObjectDecorator
    {
        private readonly BoxCollider _boxCollider;
        private bool _activated = false;

        public StraightAndBackwardImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats)
            : base(inner, mono, stats)
        {
            _boxCollider = Mono.GetComponent<BoxCollider>();
        }

        public override void Impact(Vector3 direction)
        {
            base.Impact(direction);

            _activated = direction == Vector3.forward || direction == Vector3.back;
            if (!_activated) return;

            // World-space half extents (thin on Z)
            Vector3 halfExtents = new Vector3(
                _boxCollider.bounds.extents.x,
                _boxCollider.bounds.extents.y,
                0f
            );

            float castDistance = _boxCollider.bounds.extents.z + Stats.bufferForRaycast;

            // Offset origin outward to avoid starting inside collider
            Vector3 origin = _boxCollider.bounds.center + direction * (halfExtents.z + 0.001f);

            bool blocked = Physics.BoxCast(
                origin,
                halfExtents,
                direction,
                Quaternion.identity, // World space
                castDistance,
                Stats.impactObjectLayerMask | Stats.objectBorderLayerMask
            );

            if (!blocked)
            {
                Mono.transform.Translate(direction * Stats.speed * Time.deltaTime, Space.World);
            }
            else
            {
                Debug.Log("Blocked by object. No movement (Forward/Backward).");
            }
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if (!_activated) return;

            Vector3 center = _boxCollider.bounds.center;
            Vector3 halfExtents = new Vector3(
                _boxCollider.bounds.extents.x,
                _boxCollider.bounds.extents.y,
                0f
            );

            float castDistance = _boxCollider.bounds.extents.z + Stats.bufferForRaycast;
            Vector3 boxSize = halfExtents * 2f;

            Gizmos.color = Color.red;

            // Forward
            Gizmos.matrix = Matrix4x4.TRS(center + Vector3.forward * castDistance, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // Backward
            Gizmos.matrix = Matrix4x4.TRS(center + Vector3.back * castDistance, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
