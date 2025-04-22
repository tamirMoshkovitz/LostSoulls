using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class TwoBlockHorizontalGridImpactObject : ImpactObjectDecorator
    {
        private readonly GridVisualizer _gridVisualizer;
        private readonly BoxCollider _boxCollider;
        private List<Vector3> _lastSnappedFootprint = new();

        public TwoBlockHorizontalGridImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats, GridVisualizer grid)
            : base(inner, mono, stats)
        {
            _gridVisualizer = grid;
            _boxCollider = mono.GetComponent<BoxCollider>();
        }

        public override void StartImpact()
        {
            base.StartImpact();
            if (_gridVisualizer == null) return;

            _gridVisualizer.UnmarkOccupied(Mono.GetBottomCenter(), ImpactObjectTypes.TwoBlockHorizontalGrid);
        }

        public override void StopImpact()
        {
            base.StopImpact();
            SnapToNearestGridPoint();

            if (_gridVisualizer != null && _lastSnappedFootprint.Count == 2)
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

            var footprint = _gridVisualizer.GetGridFootprint(basePosition, ImpactObjectTypes.TwoBlockHorizontalGrid);
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

            Debug.Log($"[GridSnap] {Mono.name} snapped to midpoint between horizontal grid points:\n- {footprint[0]}\n- {footprint[1]}");
        }
    }
}
