using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Collections.Generic;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class ThreeBlockHorizontalGridImpactObject : ImpactObjectDecorator
    {
        private readonly Grid grid;
        private readonly BoxCollider _boxCollider;
        private List<Vector3> _lastSnappedFootprint = new();

        public ThreeBlockHorizontalGridImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats,
            Grid grid)
            : base(inner, mono, stats)
        {
            this.grid = grid;
            _boxCollider = mono.GetComponent<BoxCollider>();
        }

        public override void StartImpact()
        {
            base.StartImpact();
            if (grid == null) return;
            grid.UnmarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.ThreeBlockHorizontalGrid);
        }

        public override void StopImpact()
        {
            base.StopImpact();
            SnapToNearestGridPoint();

            if (grid != null && _lastSnappedFootprint.Count == 3)
            {
                foreach (var cell in _lastSnappedFootprint)
                {
                    grid.MarkOccupied(cell, ImpactObjectTypes.OneBlockGrid);
                }
            }
        }

        private void SnapToNearestGridPoint()
        {
            if (grid == null || _boxCollider == null) return;

            Bounds bounds = _boxCollider.bounds;
            Vector3 basePosition = bounds.center;
            basePosition.y = bounds.min.y;

            var footprint = grid.GetGridFootprint(basePosition, ImpactObjectTypes.ThreeBlockHorizontalGrid);
            if (footprint.Count < 3)
            {
                Debug.LogWarning($"[GridSnap] Invalid 3-block horizontal footprint for {Mono.name}, skipping snap.");
                return;
            }

            _lastSnappedFootprint = footprint;

            Vector3 midpoint = (footprint[0] + footprint[1] + footprint[2]) / 3f;
            float offsetY = bounds.extents.y;

            Mono.transform.position = new Vector3(midpoint.x, midpoint.y + offsetY, midpoint.z);

            Debug.Log($"[GridSnap] {Mono.name} snapped to midpoint of 3-block horizontal grid points.");
        }
    }
}
