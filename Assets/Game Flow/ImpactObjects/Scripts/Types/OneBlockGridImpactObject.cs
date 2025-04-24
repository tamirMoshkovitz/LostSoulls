using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Linq;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class OneBlockGridImpactObject : ImpactObjectDecorator
    {
        private readonly Grid grid;
        private readonly BoxCollider _boxCollider;

        public OneBlockGridImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats, Grid grid)
            : base(inner, mono, stats)
        {
            this.grid = grid;
            _boxCollider = mono.GetComponent<BoxCollider>();
        }

        public override void StartImpact()
        {
            base.StartImpact();

            if (grid == null) return;
            grid.UnmarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.OneBlockGrid);
        }

        public override void StopImpact()
        {
            base.StopImpact();
            SnapToNearestGridPoint();

            if (grid == null) return;
            grid.MarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.OneBlockGrid);
        }

        private void SnapToNearestGridPoint()
        {
            if (grid == null || _boxCollider == null) return;

            Bounds bounds = _boxCollider.bounds;

            // Bottom center of the object
            Vector3 currentPosition = bounds.center;
            currentPosition.y = bounds.min.y;

            Vector3 closestGridPoint = grid
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
