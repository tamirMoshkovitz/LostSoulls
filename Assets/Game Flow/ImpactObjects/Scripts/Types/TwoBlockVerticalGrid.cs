using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Collections.Generic;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class TwoBlockVerticalGridImpactObject : ImpactObjectDecorator
    {
        private readonly Grid grid;
        private readonly float initialTimePerMove;

        public TwoBlockVerticalGridImpactObject(
            IImpactObject inner,
            MonoImpactObject mono,
            ImpactObjectStats stats,
            Grid grid
        ) : base(inner, mono, stats)
        {
            this.grid = grid;
            initialTimePerMove = mono.TimePerMove;
        }

        public override void UpdateImpact(Vector3 direction)
        {
            base.UpdateImpact(direction);

            // accumulate delta‐time until we should step
            Mono.TimePerMove -= Time.deltaTime;
            if (grid == null || Mono.TimePerMove >= 0f)
                return;
            Mono.TimePerMove = initialTimePerMove;

            // clear previous occupancy
            grid.UnmarkOccupied(Mono);

            // start from the cells the MonoImpactObject last occupied
            var oldCells = new List<(int row, int col)>(Mono.UsedCells);
            var targetCells = new List<(int row, int col)>();

            // compute each cell's new row/col
            foreach (var (row, col) in oldCells)
            {
                var newCell = (row, col);
                if (direction == Vector3.forward)   newCell.row += 1;
                else if (direction == Vector3.back) newCell.row -= 1;
                else if (direction == Vector3.right)newCell.col += 1;
                else if (direction == Vector3.left) newCell.col -= 1;

                targetCells.Add(newCell);
            }

            // if any target is out of bounds or occupied, block
            if (grid.IsCellsOccupied(targetCells))
            {
                Mono.IsBlocked = true;
                // put old occupancy back so grid stays consistent
                grid.MarkOccupied(Mono, oldCells);
                return;
            }
            // we can move: mark new cells, reposition, save state
            Vector3 newPos = grid.MarkOccupied(Mono, targetCells);
            Mono.transform.position = newPos;
            Mono.UsedCells = targetCells;
        }

        public override void StopImpact()
        {
            base.StopImpact();
            // no additional snapping needed
        }
    }
}
