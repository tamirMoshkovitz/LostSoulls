using System.Collections.Generic;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    /// <summary>
    /// Decorator for an “L”-shaped 3-cell object:
    ///   [X]
    /// [X][X]
    /// Cells are provided in Mono.UsedCells as three (row,col) entries forming that shape.
    /// </summary>
    public class LShapeImpactObject : ImpactObjectDecorator
    {
        private readonly Grid _grid;
        private readonly float _initialTimePerMove;

        public LShapeImpactObject(
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

            // count down until we should take one grid‐step
            Mono.TimePerMove -= Time.deltaTime;
            if (_grid == null || Mono.TimePerMove >= 0f)
                return;
            Mono.TimePerMove = _initialTimePerMove;

            // clear out the old L‐shape cells
            _grid.UnmarkOccupied(Mono);

            // figure out row/col offset from input
            int dRow = 0, dCol = 0;
            if (direction == Vector3.forward)   dRow = +1;
            else if (direction == Vector3.back) dRow = -1;
            else if (direction == Vector3.right) dCol = +1;
            else if (direction == Vector3.left)  dCol = -1;

            // compute the three target cells by shifting each old cell
            var oldCells    = new List<(int row, int col)>(Mono.UsedCells);
            var targetCells = new List<(int row, int col)>();
            foreach (var (r, c) in oldCells)
                targetCells.Add((r + dRow, c + dCol));

            // if any of the new cells is invalid or occupied, block and restore
            if (_grid.IsCellsOccupied(targetCells))
            {
                Mono.IsBlocked = true;
                _grid.MarkOccupied(Mono, oldCells);
                return;
            }
            
            // occupy the new L shape, move to its center, and save state
            Vector3 worldCenter = _grid.MarkOccupied(Mono, targetCells);
            Mono.transform.position = worldCenter;
            Mono.UsedCells = targetCells;
        }
    }
}
