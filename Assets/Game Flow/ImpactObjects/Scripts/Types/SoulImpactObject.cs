using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class SoulImpactObject : ImpactObjectDecorator
    {
        private readonly BoxCollider _boxCollider;

        public SoulImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats)
            : base(inner, mono, stats)
        {
            _boxCollider = Mono.GetComponent<BoxCollider>();
        }

        public override void Impact(Vector3 direction)
        {
            base.Impact(direction);

            Vector3 center = _boxCollider.bounds.center;

            // We'll check all 4 cardinal directions
            Vector3[] directions = new[]
            {
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left
            };

            foreach (Vector3 dir in directions)
            {
                Vector3 halfExtents;
                float castDistance;

                if (dir == Vector3.forward || dir == Vector3.back)
                {
                    // Box is thin in Z (moving forward/backward)
                    halfExtents = new Vector3(
                        _boxCollider.bounds.extents.x,
                        _boxCollider.bounds.extents.y,
                        0f
                    );
                    castDistance = _boxCollider.bounds.extents.z + Stats.bufferForRaycast;
                }
                else
                {
                    // Box is thin in X (moving left/right)
                    halfExtents = new Vector3(
                        0f,
                        _boxCollider.bounds.extents.y,
                        _boxCollider.bounds.extents.z
                    );
                    castDistance = _boxCollider.bounds.extents.x + Stats.bufferForRaycast;
                }

                Vector3 origin = center + dir * (0.001f + halfExtents.magnitude * 0.25f); // tiny offset to not be inside collider

                if (Physics.BoxCast(
                    origin,
                    halfExtents,
                    dir,
                    out RaycastHit hit,
                    Quaternion.identity,
                    castDistance,
                    Stats.impactObjectLayerMask | Stats.objectBorderLayerMask
                ))
                {
                    MonoImpactObject other = hit.collider.GetComponent<MonoImpactObject>();
                    if (other != null && other.IsSoul)
                    {
                        Debug.Log("You won");
                        break;
                    }
                }
            }
        }
    }
}
