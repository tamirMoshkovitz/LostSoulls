using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Collections.Generic;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class FourBlocksSquareGridImpactObject : ImpactObjectDecorator
    {
        private readonly Grid grid;
        private readonly BoxCollider _boxCollider;
        private List<Vector3> _lastSnappedFootprint = new();

        public FourBlocksSquareGridImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats, Grid grid)
            : base(inner, mono, stats)
        {
            this.grid = grid;
            _boxCollider = mono.GetComponent<BoxCollider>();
        }

        public override void StartImpact()
        {
            base.StartImpact();
            if (grid == null) return;
            grid.UnmarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.FourBlocksSquareGrid);
        }

        public override void StopImpact()
        {
            base.StopImpact();
            SnapToNearestGridPoint();

            if (grid != null && _lastSnappedFootprint.Count == 4)
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

            var footprint = grid.GetGridFootprint(basePosition, ImpactObjectTypes.FourBlocksSquareGrid);
            if (footprint.Count < 4)
            {
                Debug.LogWarning($"[GridSnap] Invalid 4-block square footprint for {Mono.name}, skipping snap.");
                return;
            }

            _lastSnappedFootprint = footprint;

            Vector3 center = (footprint[0] + footprint[1] + footprint[2] + footprint[3]) / 4f;
            float offsetY = bounds.extents.y;

            Mono.transform.position = new Vector3(center.x, center.y + offsetY, center.z);

            Debug.Log($"[GridSnap] {Mono.name} snapped to center of 4-block square footprint.");
        }
    }
}
