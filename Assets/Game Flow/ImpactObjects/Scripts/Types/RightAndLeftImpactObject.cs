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

            Vector3 origin = _boxCollider.bounds.center;

            // Custom halfExtents: height (Y) and depth (Z), width (X) set to 0
            Vector3 halfExtents = new Vector3(
                0f,
                _boxCollider.size.y * 0.5f,
                _boxCollider.size.z * 0.5f
            );

            float castDistance = (_boxCollider.size.x * 0.5f) + Stats.bufferForRaycast;

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
                Debug.Log("Blocked by object. No movement (Right/Left).");
            }
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if (!_activated) return;

            Vector3 origin = _boxCollider.bounds.center;
            Quaternion rotation = Mono.transform.rotation;
            float castDistance = (_boxCollider.size.x * 0.5f) + Stats.bufferForRaycast;

            // Thin X slice for Gizmo box
            Vector3 boxSize = new Vector3(
                0.01f,
                _boxCollider.size.y,
                _boxCollider.size.z
            );

            Gizmos.color = Color.blue;

            // Right box
            Gizmos.matrix = Matrix4x4.TRS(origin + Vector3.right * castDistance, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // Left box
            Gizmos.matrix = Matrix4x4.TRS(origin + Vector3.left * castDistance, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
