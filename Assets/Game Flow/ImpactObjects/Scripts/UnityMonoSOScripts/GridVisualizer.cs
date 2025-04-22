using System.Collections.Generic;
using System.Linq;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using UnityEngine;
using static Game_Flow.ImpactObjects.Scripts.Decorator_Interface.ImpactObjectTypes;

namespace Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts
{
    [ExecuteAlways]
    public class GridVisualizer : MonoBehaviour
    {
        [SerializeField] private int rows = 6;
        [SerializeField] private int cols = 4;
        [SerializeField] private float gizmoRadius = 0.05f;
        [SerializeField] private float gizmoHeightOffset = 0.1f;

        private List<Vector3> allGridCenters = new();
        private HashSet<Vector3> occupiedCenters = new();
        private Renderer _renderer;

        public List<Vector3> AllGridCenters => new(allGridCenters);

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            CalculateGridCenters();
        }
        

        void CalculateGridCenters()
        {
            allGridCenters.Clear();

            Bounds bounds = _renderer.bounds;
            Vector3 size = bounds.size;
            Vector3 origin = bounds.min;

            float cellWidth = size.x / cols;
            float cellHeight = size.z / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    float x = origin.x + cellWidth * col + cellWidth / 2f;
                    float z = origin.z + cellHeight * row + cellHeight / 2f;
                    float y = bounds.center.y;
                    allGridCenters.Add(new Vector3(x, y, z));
                }
            }
        }

        public void MarkOccupied(Vector3 basePosition, ImpactObjectTypes shape)
        {
            foreach (var cell in GetGridFootprint(basePosition, shape))
            {
                occupiedCenters.Add(cell);
            }
        }

        public void UnmarkOccupied(Vector3 basePosition, ImpactObjectTypes shape)
        {
            foreach (var cell in GetGridFootprint(basePosition, shape))
            {
                occupiedCenters.Remove(cell);
            }
        }

        public bool IsOccupied(Vector3 gridPosition)
        {
            return occupiedCenters.Contains(gridPosition);
        }

        public List<Vector3> GetAvailableGridCenters()
        {
            return allGridCenters
                .Where(p => !occupiedCenters.Contains(p))
                .ToList();
        }

        public List<Vector3> GetGridFootprint(Vector3 basePos, ImpactObjectTypes shape)
        {
            List<Vector3> result = new();
            
            Bounds bounds = _renderer.bounds;
            float cellWidth = bounds.size.x / cols;
            float cellHeight = bounds.size.z / rows;

            switch (shape)
            {
                case OneBlockGrid:
                    result.Add(FindClosestGridPoint(basePos));
                    break;

                case TwoBlockHorizontalGrid:
                    result.Add(FindClosestGridPoint(basePos + Vector3.left * cellWidth / 2));
                    result.Add(FindClosestGridPoint(basePos + Vector3.right * cellWidth / 2));
                    break;

                case TwoBlockVerticalGrid:
                    result.Add(FindClosestGridPoint(basePos + Vector3.back * cellHeight / 2));
                    result.Add(FindClosestGridPoint(basePos + Vector3.forward * cellHeight / 2));
                    break;

                case ThreeBlockHorizontalGrid:
                    result.Add(FindClosestGridPoint(basePos));
                    result.Add(FindClosestGridPoint(basePos + Vector3.left * cellWidth));
                    result.Add(FindClosestGridPoint(basePos + Vector3.right * cellWidth));
                    break;

                case ThreeBlockVerticalGrid:
                    result.Add(FindClosestGridPoint(basePos));
                    result.Add(FindClosestGridPoint(basePos + Vector3.back * cellHeight));
                    result.Add(FindClosestGridPoint(basePos + Vector3.forward * cellHeight));
                    break;

                case FourBlocksSquareGrid:
                    result.Add(FindClosestGridPoint(basePos + new Vector3(-cellWidth / 2, 0, -cellHeight / 2)));
                    result.Add(FindClosestGridPoint(basePos + new Vector3(cellWidth / 2, 0, -cellHeight / 2)));
                    result.Add(FindClosestGridPoint(basePos + new Vector3(-cellWidth / 2, 0, cellHeight / 2)));
                    result.Add(FindClosestGridPoint(basePos + new Vector3(cellWidth / 2, 0, cellHeight / 2)));
                    break;
            }

            return result.Distinct().ToList();
        }

        private Vector3 FindClosestGridPoint(Vector3 pos)
        {
            return allGridCenters
                .OrderBy(p => Vector3.Distance(pos, p))
                .FirstOrDefault();
        }

        void OnDrawGizmos()
        {
            if (allGridCenters == null || allGridCenters.Count == 0)
                    CalculateGridCenters(); // Ensures gizmos appear in editor
            
            foreach (Vector3 center in allGridCenters)
            {
                Gizmos.color = occupiedCenters.Contains(center) ? Color.green : Color.red;
        
                Vector3 drawPosition = center + Vector3.up * gizmoHeightOffset;
                Gizmos.DrawSphere(drawPosition, gizmoRadius);
            }
        }
    }
}