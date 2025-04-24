using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class RightImpactObject : ImpactObjectDecorator
    {
        private readonly BoxCollider _boxCollider;
        private bool _activated = false;

        public RightImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats)
            : base(inner, mono, stats)
        {
            _boxCollider = Mono.GetComponent<BoxCollider>();
        }

        public override void UpdateImpact(Vector3 direction)
        {
            base.UpdateImpact(direction);

            _activated = direction == Vector3.right;
            if (!_activated) return;

            Vector3 halfExtents = new Vector3(0f, _boxCollider.bounds.extents.y, _boxCollider.bounds.extents.z);
            float castDistance = _boxCollider.bounds.extents.x + Stats.bufferForRaycast;
            Vector3 origin = _boxCollider.bounds.center + Vector3.right * (halfExtents.x + 0.001f);

            RaycastHit hit;
            bool hitSomething = Physics.BoxCast(
                origin,
                halfExtents,
                Vector3.right,
                out hit,
                Quaternion.identity,
                castDistance,
                Stats.impactObjectLayerMask | Stats.objectBorderLayerMask
            );

            bool blocked = false;

            if (hitSomething)
            {
                var other = hit.collider.GetComponent<MonoImpactObject>();
                if (other == null || !Mono.NonCollidingObjects.Contains(other))
                {
                    blocked = true;
                }
            }
            if (!blocked)
            {
                Mono.transform.Translate(Vector3.right * Stats.speed * Time.deltaTime, Space.World);
            }
            else
            {
                Mono.IsBlocked = true;
                Debug.Log("Blocked (Right)");
            }
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if (!_activated) return;

            Vector3 halfExtents = new Vector3(0f, _boxCollider.bounds.extents.y, _boxCollider.bounds.extents.z);
            float castDistance = _boxCollider.bounds.extents.x + Stats.bufferForRaycast;
            Vector3 boxSize = halfExtents * 2f;
            Vector3 origin = _boxCollider.bounds.center + Vector3.right * castDistance;

            Gizmos.color = Color.blue;
            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
