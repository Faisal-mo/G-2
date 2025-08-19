using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid Settings")]
    public float cellSize = 1.0f;       // Size of each grid cell
    public int gridWidth = 20;          // Cells per player side
    public int gridDepth = 10;          // Total grid depth
    public float centerGap = 2.0f;      // Space between player zones

    [Header("Debug")]
    public bool showOccupiedCells = true;

    private Dictionary<Vector3Int, bool> occupiedCells = new Dictionary<Vector3Int, bool>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Convert world position to exact grid cell center
    public Vector3 SnapToGrid(Vector3 worldPos)
    {
        Vector3Int cell = WorldToCell(worldPos);
        return new Vector3(
            cell.x * cellSize + cellSize * 0.5f,
            0,
            cell.z * cellSize + cellSize * 0.5f
        );
    }

    // World position to grid coordinates
    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return new Vector3Int(
            Mathf.FloorToInt(worldPos.x / cellSize),
            0,
            Mathf.FloorToInt(worldPos.z / cellSize)
        );
    }

    // Check if cell is available
    public bool IsCellEmpty(Vector3 worldPos)
    {
        Vector3Int cell = WorldToCell(worldPos);
        return !occupiedCells.ContainsKey(cell);
    }

    // Reserve a cell
    public void OccupyCell(Vector3 worldPos)
    {
        Vector3Int cell = WorldToCell(worldPos);
        occupiedCells[cell] = true;
    }

    // Get valid build position for player
    public Vector3 GetRandomBuildPosition(int playerID)
    {
        int maxAttempts = 30; // Increased from 10 for better results
        float halfWidth = (gridWidth * cellSize + centerGap) * 0.5f;

        for (int i = 0; i < maxAttempts; i++)
        {
            // Calculate player-specific X range
            float xMin = (playerID == 1) ? -halfWidth : centerGap * 0.5f;
            float xMax = (playerID == 1) ? -centerGap * 0.5f : halfWidth;

            Vector3 pos = new Vector3(
                Random.Range(xMin, xMax),
                0,
                Random.Range(-gridDepth * 0.5f * cellSize, gridDepth * 0.5f * cellSize)
            );

            Vector3 snappedPos = SnapToGrid(pos);
            if (IsCellEmpty(snappedPos)) return snappedPos;
        }
        Debug.LogWarning($"Failed to find valid position after {maxAttempts} attempts");
        return Vector3.zero;
    }

    // Visual debugging
    void OnDrawGizmos()
    {
        float halfTotalWidth = (gridWidth * cellSize + centerGap) * 0.5f;
        float halfGap = centerGap * 0.5f;

        // Player 1 Grid (Left - Blue)
        Gizmos.color = new Color(0, 0.5f, 1f, 0.3f);
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 pos = new Vector3(
                    x * cellSize - halfTotalWidth + cellSize * 0.5f,
                    0,
                    z * cellSize - (gridDepth * 0.5f * cellSize) + cellSize * 0.5f
                );
                Gizmos.DrawWireCube(pos, Vector3.one * cellSize * 0.95f);
            }
        }

        // Player 2 Grid (Right - Red)
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.3f);
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 pos = new Vector3(
                    x * cellSize + halfGap + cellSize * 0.5f,
                    0,
                    z * cellSize - (gridDepth * 0.5f * cellSize) + cellSize * 0.5f
                );
                Gizmos.DrawWireCube(pos, Vector3.one * cellSize * 0.95f);
            }
        }

        // Occupied cells (Magenta)
        if (showOccupiedCells && Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            foreach (var cell in occupiedCells.Keys)
            {
                Vector3 center = new Vector3(
                    cell.x * cellSize + cellSize * 0.5f,
                    0,
                    cell.z * cellSize + cellSize * 0.5f
                );
                Gizmos.DrawCube(center, Vector3.one * cellSize * 0.8f);
            }
        }
    }
}