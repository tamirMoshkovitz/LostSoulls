using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class RightAndLeftImpactObject : ImpactObjectDecorator
    {
        private readonly BoxCollider _boxCollider;
        private bool _activated = false;

        public RightAndLeftImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats)
            : base(inner, mono, stats)
        {
            _boxCollider = Mono.GetComponent<BoxCollider>();
        }

        public override void Impact(Vector3 direction)
        {
            base.Impact(direction);

            _activated = direction == Vector3.right || direction == Vector3.left;
            if (!_activated) return;

            // Get safe cast origin & world-space box size
            Vector3 halfExtents = new Vector3(0f, _boxCollider.bounds.extents.y, _boxCollider.bounds.extents.z);
            float castDistance = _boxCollider.bounds.extents.x + Stats.bufferForRaycast;
            Vector3 origin = _boxCollider.bounds.center + direction * (halfExtents.x + 0.001f); // shift slightly outside collider

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
                Debug.Log("Blocked by object. No movement (Right/Left).");
            }
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if (!_activated) return;

            Vector3 center = _boxCollider.bounds.center;
            Vector3 halfExtents = new Vector3(0f, _boxCollider.bounds.extents.y, _boxCollider.bounds.extents.z);
            float castDistance = _boxCollider.bounds.extents.x + Stats.bufferForRaycast;
            Vector3 boxSize = halfExtents * 2f;

            Gizmos.color = Color.blue;

            // Right
            Gizmos.matrix = Matrix4x4.TRS(center + Vector3.right * castDistance, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // Left
            Gizmos.matrix = Matrix4x4.TRS(center + Vector3.left * castDistance, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
