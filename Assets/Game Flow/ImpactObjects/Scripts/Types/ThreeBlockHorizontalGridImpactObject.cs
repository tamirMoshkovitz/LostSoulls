using System.Collections.Generic;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class ThreeBlockHorizontalGridImpactObject : ImpactObjectDecorator
    {
        private readonly Grid _grid;
        private readonly float _initialTimePerMove;

        public ThreeBlockHorizontalGridImpactObject(
            IImpactObject inner,
            MonoImpactObject mono,
            ImpactObjectStats stats,
            Grid grid
        ) : base(inner, mono, stats)
        {
            _grid = grid;
            _initialTimePerMove = mono.TimePerMove;
        }

        public override void UpdateImpact(Vector3 direction)
        {
            base.UpdateImpact(direction);

            // accumulate until a discrete step is due
            Mono.TimePerMove -= Time.deltaTime;
            if (_grid == null || Mono.TimePerMove >= 0f)
                return;
            Mono.TimePerMove = _initialTimePerMove;

            // clear out the old three‐cell footprint
            _grid.UnmarkOccupied(Mono);

            // compute row/col offset
            int dRow = 0, dCol = 0;
            if (direction == Vector3.forward)    dRow = +1;
            else if (direction == Vector3.back)  dRow = -1;
            else if (direction == Vector3.right) dCol = +1;
            else if (direction == Vector3.left)  dCol = -1;

            // shift each occupied cell
            var oldCells    = new List<(int row, int col)>(Mono.UsedCells);
            var targetCells = new List<(int row, int col)>();
            foreach (var (r, c) in oldCells)
                targetCells.Add((r + dRow, c + dCol));

            // if any new cell is invalid or occupied, revert and block
            if (_grid.IsCellsOccupied(targetCells))
            {
                Mono.IsBlocked = true;
                _grid.MarkOccupied(Mono, oldCells);
                return;
            }
            
            // otherwise occupy new cells and move to their center
            Vector3 worldCenter = _grid.MarkOccupied(Mono, targetCells);
            Mono.transform.position = worldCenter;
            Mono.UsedCells = targetCells;
        }
    }
}
