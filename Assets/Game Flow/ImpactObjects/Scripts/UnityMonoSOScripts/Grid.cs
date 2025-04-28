using System.Collections.Generic;
using UnityEngine;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using static Game_Flow.ImpactObjects.Scripts.Decorator_Interface.ImpactObjectTypes;

namespace Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts
{
    [ExecuteAlways]
    public class Grid : MonoBehaviour
    {
        [SerializeField] private int rows = 6;
        [SerializeField] private int cols = 4;
        [SerializeField] private float gizmoRadius = 0.05f;
        [SerializeField] private float gizmoHeightOffset = 0.1f;

        public int Cols => cols;
        public int Rows => rows;

        private List<List<Vector3>> allGridCenters = new();
        private List<List<MonoImpactObject>> occupiedCenters = new();
        private Renderer _renderer;

        public List<List<Vector3>> AllGridCenters => new(allGridCenters);

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            CalculateGridCenters();
        }

        private void CalculateGridCenters()
        {
            allGridCenters.Clear();
            occupiedCenters.Clear();

            Bounds bounds = _renderer.bounds;
            Vector3 size = bounds.size;
            Vector3 origin = bounds.min;

            float cellWidth = size.x / cols;
            float cellHeight = size.z / rows;

            for (int row = 0; row < rows; row++)
            {
                var rowCenters = new List<Vector3>();
                var rowOccupied = new List<MonoImpactObject>();

                for (int col = 0; col < cols; col++)
                {
                    float x = origin.x + cellWidth * col + cellWidth / 2f;
                    float z = origin.z + cellHeight * row + cellHeight / 2f;
                    float y = bounds.center.y;
                    rowCenters.Add(new Vector3(x, y, z));
                    rowOccupied.Add(null); // Initialize as empty
                }

                allGridCenters.Add(rowCenters);
                occupiedCenters.Add(rowOccupied);
            }
        }

        private bool IsValidCell(int row, int col)
        {
            return row >= 0 && row < rows && col >= 0 && col < cols;
        }

        /// <summary>
        /// Marks the given grid cells as occupied by the given MonoImpactObject and returns the center position of the occupied area.
        /// </summary>
        public Vector3 MarkOccupied(MonoImpactObject impactObject, List<(int row, int col)> gridCells)
        {
            float minX = float.MaxValue, minZ = float.MaxValue;
            float maxX = float.MinValue, maxZ = float.MinValue;
            float y = 0f;

            foreach (var (row, col) in gridCells)
            {
                if (IsValidCell(row, col))
                {
                    Vector3 cellCenter = allGridCenters[row][col];
                    occupiedCenters[row][col] = impactObject;
                    minX = Mathf.Min(minX, cellCenter.x);
                    maxX = Mathf.Max(maxX, cellCenter.x);
                    minZ = Mathf.Min(minZ, cellCenter.z);
                    maxZ = Mathf.Max(maxZ, cellCenter.z);
                    y = cellCenter.y;
                }
            }

            return new Vector3((minX + maxX) / 2f, y, (minZ + maxZ) / 2f);
        }

        /// <summary>
        /// Unmarks all cells currently occupied by the given MonoImpactObject.
        /// </summary>
        public void UnmarkOccupied(MonoImpactObject impactObject)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (occupiedCenters[row][col] == impactObject)
                    {
                        occupiedCenters[row][col] = null;
                    }
                }
            }
        }

        public bool IsCellsOccupied(List<(int row, int col)> gridCells)
        {
            foreach (var (row, col) in gridCells)
            {
                if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                    return true; // out of bounds = considered occupied

                if (occupiedCenters[row][col] != null)
                    return true; // occupied by another object
            }

            return false; // all cells free
        }

        void OnDrawGizmos()
        {
            if (allGridCenters == null || allGridCenters.Count == 0)
                CalculateGridCenters();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Vector3 center = allGridCenters[row][col];
                    bool isOccupied = occupiedCenters.Count > row && occupiedCenters[row].Count > col && occupiedCenters[row][col] != null;
                    Gizmos.color = isOccupied ? Color.green : Color.red;

                    Vector3 drawPosition = center + Vector3.up * gizmoHeightOffset;
                    Gizmos.DrawSphere(drawPosition, gizmoRadius);
                }
            }
        }
    }
}
