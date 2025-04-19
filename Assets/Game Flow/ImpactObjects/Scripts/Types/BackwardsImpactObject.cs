using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class BackwardImpactObject : ImpactObjectDecorator
    {
        private readonly BoxCollider _boxCollider;
        private bool _activated = false;

        public BackwardImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats)
            : base(inner, mono, stats)
        {
            _boxCollider = Mono.GetComponent<BoxCollider>();
        }

        public override void Impact(Vector3 direction)
        {
            base.Impact(direction);
            _activated = direction == Vector3.back;
            if (!_activated) return;

            Vector3 halfExtents = new Vector3(
                _boxCollider.bounds.extents.x,
                _boxCollider.bounds.extents.y,
                0f
            );

            float castDistance = _boxCollider.bounds.extents.z + Stats.bufferForRaycast;
            Vector3 origin = _boxCollider.bounds.center + Vector3.back * (halfExtents.z + 0.001f);

            bool blocked = Physics.BoxCast(
                origin,
                halfExtents,
                Vector3.back,
                Quaternion.identity,
                castDistance,
                Stats.impactObjectLayerMask | Stats.objectBorderLayerMask
            );

            if (!blocked)
            {
                Mono.transform.Translate(Vector3.back * Stats.speed * Time.deltaTime, Space.World);
            }
            else
            {
                Mono.IsBlocked = true;
                Debug.Log("Blocked (Backward)");
            }
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if (!_activated) return;

            Vector3 halfExtents = new Vector3(
                _boxCollider.bounds.extents.x,
                _boxCollider.bounds.extents.y,
                0f
            );

            float castDistance = _boxCollider.bounds.extents.z + Stats.bufferForRaycast;
            Vector3 boxSize = halfExtents * 2f;
            Vector3 origin = _boxCollider.bounds.center + Vector3.back * castDistance;

            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
