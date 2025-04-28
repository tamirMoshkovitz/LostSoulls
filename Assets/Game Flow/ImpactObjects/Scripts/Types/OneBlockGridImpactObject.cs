using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class OneBlockGridImpactObject : ImpactObjectDecorator
    {
        private readonly Grid grid;
        private (int row, int col) currentCell;
        private readonly float intialTimePersume;


        public OneBlockGridImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats, Grid grid)
            : base(inner, mono, stats)
        {
            currentCell = Mono.UsedCells.First();
            intialTimePersume = Mono.TimePerMove;
            this.grid = grid;
        }
        
        public override void UpdateImpact(Vector3 direction)
        {
            base.UpdateImpact(direction);
            Mono.TimePerMove -= Time.deltaTime;
            if (grid == null || Mono.TimePerMove >= 0) return;
            Mono.TimePerMove = intialTimePersume;
            grid.UnmarkOccupied(Mono);
            (int row, int col) targetCell = currentCell;

            // Which direction?
            if (direction == Vector3.left)
                targetCell.col -= 1;
            else if (direction == Vector3.right)
                targetCell.col += 1;
            else if (direction == Vector3.forward)
                targetCell.row += 1;
            else if (direction == Vector3.back)
                targetCell.row -= 1;

            // Check bounds
            if (targetCell.row < 0 || targetCell.row >= grid.Rows || targetCell.col < 0 || targetCell.col >= grid.Cols)
                return;
            List<(int row, int col)> cells = new List<(int row, int col)> { (targetCell.row, targetCell.col) };
            // Check if target cell is free
            if (!grid.IsCellsOccupied(cells))
            {
                // Move
                
                Vector3 newPosition = grid.MarkOccupied(Mono, cells);
                Mono.transform.position = newPosition;

                // Update grid
                Mono.UsedCells = cells;
                currentCell = targetCell;
            }
            grid.MarkOccupied(Mono, Mono.UsedCells);
        }
        
    }
}
