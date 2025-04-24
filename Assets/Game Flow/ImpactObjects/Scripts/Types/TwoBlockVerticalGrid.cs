using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class TwoBlockVerticalGridImpactObject : ImpactObjectDecorator
    {
        private readonly Grid grid;
        private readonly BoxCollider _boxCollider;
        private List<Vector3> _lastSnappedFootprint = new();

        public TwoBlockVerticalGridImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats, Grid grid)
            : base(inner, mono, stats)
        {
            this.grid = grid;
            _boxCollider = mono.GetComponent<BoxCollider>();
        }

        public override void StartImpact()
        {
            base.StartImpact();
            if (grid == null) return;

            grid.UnmarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.TwoBlockVerticalGrid);
        }

        public override void StopImpact()
        {
            base.StopImpact();
            SnapToNearestGridPoint();

            if (grid != null && _lastSnappedFootprint.Count == 2)
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

            var footprint = grid.GetGridFootprint(basePosition, ImpactObjectTypes.TwoBlockVerticalGrid);
            if (footprint.Count < 2)
            {
                Debug.LogWarning($"[GridSnap] Invalid footprint for {Mono.name}, skipping snap.");
                return;
            }

            _lastSnappedFootprint = footprint;

            Vector3 midpoint = (footprint[0] + footprint[1]) * 0.5f;
            float offsetY = bounds.extents.y;

            Mono.transform.position = new Vector3(
                midpoint.x,
                midpoint.y + offsetY,
                midpoint.z
            );

            Debug.Log($"[GridSnap] {Mono.name} snapped to midpoint between vertical grid points:\n- {footprint[0]}\n- {footprint[1]}");
        }
    }
}
