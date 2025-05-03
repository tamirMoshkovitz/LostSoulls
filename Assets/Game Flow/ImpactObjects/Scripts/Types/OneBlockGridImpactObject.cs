using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using UnityEngine;
using Grid = Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts.Grid;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class OneBlockGridImpactObject : ImpactObjectDecorator
    {
        private readonly Grid                grid;
        private          (int row, int col)  currentCell;
        private          Tween               _moveTween;

        public OneBlockGridImpactObject(
            IImpactObject inner,
            MonoImpactObject mono,
            ImpactObjectStats stats,
            Grid grid
        ) : base(inner, mono, stats)
        {
            currentCell = Mono.UsedCells.First();
            this.grid   = grid;
        }

        public override void UpdateImpact(Vector3 direction)
        {
            base.UpdateImpact(direction);

            // 1) if we're mid-tween, don't start another move
            if (_moveTween != null && _moveTween.IsActive() && !_moveTween.IsComplete())
                return;

            // 2) unmark our old cell so grid.IsCellsOccupied() sees us as free
            grid.UnmarkOccupied(Mono);

            // 3) figure out the target cell
            var targetCell = currentCell;
            if      (direction == Vector3.left)    targetCell.col -= 1;
            else if (direction == Vector3.right)   targetCell.col += 1;
            else if (direction == Vector3.forward) targetCell.row += 1;
            else if (direction == Vector3.back)    targetCell.row -= 1;

            // 4) bounds check
            if (targetCell.row < 0 || targetCell.row >= grid.Rows ||
                targetCell.col < 0 || targetCell.col >= grid.Cols)
            {
                // re-mark our original spot and bail
                grid.MarkOccupied(Mono, Mono.UsedCells);
                return;
            }

            // 5) if that cell’s free, launch the DOTween move
            var cells = new List<(int, int)> { targetCell };
            if (!grid.IsCellsOccupied(cells))
            {
                Vector3 newPosition = grid.MarkOccupied(Mono, cells);
                Mono.ObjectAudio.PlaySound();

                _moveTween = Mono.transform
                    .DOMove(newPosition, Stats.timePerMove) 
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        // commit cell & re-mark
                        Mono.UsedCells = cells;
                        currentCell    = targetCell;
                        grid.MarkOccupied(Mono, Mono.UsedCells);

                        Mono.ObjectAudio.StopSound();
                        _moveTween = null;
                    });

                return;
            }

            // 6) if blocked, just re-mark and exit
            grid.MarkOccupied(Mono, Mono.UsedCells);
        }
    }
}
