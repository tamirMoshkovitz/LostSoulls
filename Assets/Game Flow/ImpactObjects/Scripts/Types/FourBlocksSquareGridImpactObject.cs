using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Collections.Generic;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class FourBlocksSquareGridImpactObject : ImpactObjectDecorator
    {
        private readonly GridVisualizer _gridVisualizer;
        private readonly BoxCollider _boxCollider;
        private List<Vector3> _lastSnappedFootprint = new();

        public FourBlocksSquareGridImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats, GridVisualizer grid)
            : base(inner, mono, stats)
        {
            _gridVisualizer = grid;
            _boxCollider = mono.GetComponent<BoxCollider>();
        }

        public override void StartImpact()
        {
            base.StartImpact();
            if (_gridVisualizer == null) return;
            _gridVisualizer.UnmarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.FourBlocksSquareGrid);
        }

        public override void StopImpact()
        {
            base.StopImpact();
            SnapToNearestGridPoint();

            if (_gridVisualizer != null && _lastSnappedFootprint.Count == 4)
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

            var footprint = _gridVisualizer.GetGridFootprint(basePosition, ImpactObjectTypes.FourBlocksSquareGrid);
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
