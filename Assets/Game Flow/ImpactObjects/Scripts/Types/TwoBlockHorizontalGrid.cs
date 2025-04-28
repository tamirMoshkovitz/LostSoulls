using System.Collections.Generic;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using static Game_Flow.ImpactObjects.Scripts.Decorator_Interface.ImpactObjectTypes;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class TwoBlockHorizontalGridImpactObject : ImpactObjectDecorator
    {
        private readonly Grid grid;
        private readonly float initialTimePerMove;

        public TwoBlockHorizontalGridImpactObject(
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

            // accumulate until it’s time to step
            Mono.TimePerMove -= Time.deltaTime;
            if (grid == null || Mono.TimePerMove >= 0f)
                return;
            Mono.TimePerMove = initialTimePerMove;

            // clear old occupancy
            grid.UnmarkOccupied(Mono);

            // figure out our row/col delta
            int dRow = 0, dCol = 0;
            if (direction == Vector3.forward)   dRow =  1;
            else if (direction == Vector3.back) dRow = -1;
            else if (direction == Vector3.right) dCol =  1;
            else if (direction == Vector3.left)  dCol = -1;

            // compute the two new target cells
            var oldCells    = new List<(int row, int col)>(Mono.UsedCells);
            var targetCells = new List<(int row, int col)>();
            foreach (var (r, c) in oldCells)
                targetCells.Add((r + dRow, c + dCol));

            // if any of them is blocked, revert and bail
            if (grid.IsCellsOccupied(targetCells))
            {
                Mono.IsBlocked = true;
                grid.MarkOccupied(Mono, oldCells);
                return;
            }
            
            // otherwise occupy & move
            Vector3 worldCenter = grid.MarkOccupied(Mono, targetCells);
            Mono.transform.position = worldCenter;
            Mono.UsedCells = targetCells;
        }
    }
}
