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

            Vector3 origin = _boxCollider.bounds.center;

            // Custom halfExtents for width (x), height (y), but not length (z)
            Vector3 halfExtents = new Vector3(
                _boxCollider.size.x * 0.5f,
                _boxCollider.size.y * 0.5f,
                0f
            );

            float castDistance = (_boxCollider.size.z * 0.5f) + Stats.bufferForRaycast;

            bool blocked = Physics.BoxCast(
                origin,
                halfExtents,
                direction, 
                Mono.transform.rotation,
                castDistance,
                Stats.impactObjectLayerMask | Stats.objectBorderLayerMask
            );

            if (!blocked)
            {
                Mono.transform.Translate(direction * Stats.speed * Time.deltaTime, Space.World);
            }
            else
            {
                Debug.Log("Blocked by object. No movement.");
            }
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if (!_activated) return;

            Vector3 origin = _boxCollider.bounds.center;
            Quaternion rotation = Mono.transform.rotation;
            float castDistance = (_boxCollider.size.z * 0.5f) + Stats.bufferForRaycast;

            Vector3 boxSize = new Vector3(
                _boxCollider.size.x,
                _boxCollider.size.y,
                0.01f // thin line for visualizing box in Z
            );

            Gizmos.color = Color.red;

            // Forward box
            Gizmos.matrix = Matrix4x4.TRS(origin + Vector3.forward * castDistance, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // Backward box
            Gizmos.matrix = Matrix4x4.TRS(origin + Vector3.back * castDistance, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
