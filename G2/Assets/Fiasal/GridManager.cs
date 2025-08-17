using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid Settings")]
    public float cellSize = 1.0f;
    public int gridWidth = 20;     // Cells per player (half total width)
    public int gridDepth = 10;     // Total grid depth
    public float centerGap = 2.0f; // Space between P1 and P2 grids (adjustable)

    [Header("Player Colors")]
    public Color player1Color = Color.cyan;
    public Color player2Color = Color.red;

    private Dictionary<Vector3Int, bool> occupiedCells = new Dictionary<Vector3Int, bool>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Convert world position to grid cell
    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        float adjustedX = worldPos.x + (gridWidth * cellSize + centerGap) * 0.5f;
        int x = Mathf.FloorToInt(adjustedX / cellSize);
        int z = Mathf.FloorToInt(worldPos.z / cellSize);
        return new Vector3Int(x, 0, z);
    }

    // Check if cell is empty
    public bool IsCellEmpty(Vector3 worldPos)
    {
        Vector3Int cell = WorldToCell(worldPos);
        return !occupiedCells.ContainsKey(cell);
    }

    // Occupy cell
    public void OccupyCell(Vector3 worldPos)
    {
        Vector3Int cell = WorldToCell(worldPos);
        occupiedCells[cell] = true;
    }

    // Get random position in player zone
    public Vector3 GetRandomBuildPosition(int playerID)
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            float xOffset = (gridWidth * cellSize + centerGap) * 0.5f;
            float x = (playerID == 1)
                ? Random.Range(-xOffset, -centerGap * 0.5f)
                : Random.Range(centerGap * 0.5f, xOffset);

            Vector3 pos = new Vector3(
                x,
                0,
                Random.Range(-gridDepth * 0.5f * cellSize, gridDepth * 0.5f * cellSize)
            );

            if (IsCellEmpty(pos)) return pos;
        }
        return Vector3.zero;
    }

    // Draw colored grid with center gap
    void OnDrawGizmos()
    {
        float halfTotalWidth = (gridWidth * cellSize + centerGap) * 0.5f;
        float halfGap = centerGap * 0.5f;

        // Player 1 Grid (Left - Cyan)
        Gizmos.color = player1Color;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 pos = new Vector3(
                    x * cellSize - halfTotalWidth,
                    0,
                    z * cellSize - (gridDepth * 0.5f * cellSize)
                );
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.1f, cellSize));
            }
        }

        // Player 2 Grid (Right - Red)
        Gizmos.color = player2Color;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 pos = new Vector3(
                    x * cellSize + halfGap,
                    0,
                    z * cellSize - (gridDepth * 0.5f * cellSize)
                );
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }
}