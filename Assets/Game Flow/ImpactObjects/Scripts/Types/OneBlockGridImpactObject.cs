using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Linq;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class OneBlockGridImpactObject : ImpactObjectDecorator
    {
        private readonly GridVisualizer _gridVisualizer;
        private readonly BoxCollider _boxCollider;

        public OneBlockGridImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats, GridVisualizer grid)
            : base(inner, mono, stats)
        {
            _gridVisualizer = grid;
            _boxCollider = mono.GetComponent<BoxCollider>();
        }

        public override void StartImpact()
        {
            base.StartImpact();

            if (_gridVisualizer == null) return;
            _gridVisualizer.UnmarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.OneBlockGrid);
        }

        public override void StopImpact()
        {
            base.StopImpact();
            SnapToNearestGridPoint();

            if (_gridVisualizer == null) return;
            _gridVisualizer.MarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.OneBlockGrid);
        }

        private void SnapToNearestGridPoint()
        {
            if (_gridVisualizer == null || _boxCollider == null) return;

            Bounds bounds = _boxCollider.bounds;

            // Bottom center of the object
            Vector3 currentPosition = bounds.center;
            currentPosition.y = bounds.min.y;

            Vector3 closestGridPoint = _gridVisualizer
                .GetAvailableGridCenters()
                .OrderBy(p => Vector3.Distance(currentPosition, p))
                .FirstOrDefault();

            // Offset so object's base aligns with grid
            float objectBottomOffset = bounds.extents.y;
            Vector3 snapPosition = new Vector3(
                closestGridPoint.x,
                closestGridPoint.y + objectBottomOffset,
                closestGridPoint.z
            );

            Mono.transform.position = snapPosition;
            Debug.Log($"[GridSnap] {Mono.name} snapped OneBlock to nearest grid point at {closestGridPoint}.");
        }
    }
}
