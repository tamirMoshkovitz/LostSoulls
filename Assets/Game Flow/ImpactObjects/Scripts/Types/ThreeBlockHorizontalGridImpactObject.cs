using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Collections.Generic;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class ThreeBlockHorizontalGridImpactObject : ImpactObjectDecorator
    {
        private readonly GridVisualizer _gridVisualizer;
        private readonly BoxCollider _boxCollider;
        private List<Vector3> _lastSnappedFootprint = new();

        public ThreeBlockHorizontalGridImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats,
            GridVisualizer grid)
            : base(inner, mono, stats)
        {
            _gridVisualizer = grid;
            _boxCollider = mono.GetComponent<BoxCollider>();
        }

        public override void StartImpact()
        {
            base.StartImpact();
            if (_gridVisualizer == null) return;
            _gridVisualizer.UnmarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.ThreeBlockHorizontalGrid);
        }

        public override void StopImpact()
        {
            base.StopImpact();
            SnapToNearestGridPoint();

            if (_gridVisualizer != null && _lastSnappedFootprint.Count == 3)
            {
                foreach (var cell in _lastSnappedFootprint)
                {
                    _gridVisualizer.MarkOccupied(cell, ImpactObjectTypes.OneBlockGrid);
                }
            }
        }

        private void SnapToNearestGridPoint()
        {
            if (_gridVisualizer == null || _boxCollider == null) return;

            Bounds bounds = _boxCollider.bounds;
            Vector3 basePosition = bounds.center;
            basePosition.y = bounds.min.y;

            var footprint = _gridVisualizer.GetGridFootprint(basePosition, ImpactObjectTypes.ThreeBlockHorizontalGrid);
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
