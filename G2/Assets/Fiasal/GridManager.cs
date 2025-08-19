using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public float cellSize = 1f;
    public int gridWidth = 20;
    public int gridDepth = 10;
    public float centerGap = 2f;

    public Color player1Color = Color.cyan;
    public Color player2Color = Color.red;

    Dictionary<Vector3Int, bool> occupied = new Dictionary<Vector3Int, bool>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        float halfTotal = gridWidth * cellSize + centerGap;
        float adjustedX = worldPos.x + halfTotal * 0.5f;
        int x = Mathf.FloorToInt(adjustedX / cellSize);
        int z = Mathf.FloorToInt((worldPos.z + gridDepth * 0.5f * cellSize) / cellSize);
        return new Vector3Int(x, 0, z);
    }

    public Vector3 CellToWorldCenter(Vector3Int cell)
    {
        float halfTotal = gridWidth * cellSize + centerGap;
        float x = cell.x * cellSize - halfTotal * 0.5f + cellSize * 0.5f;
        float z = cell.z * cellSize - gridDepth * 0.5f * cellSize + cellSize * 0.5f;
        return new Vector3(x, 0f, z);
    }

    public bool IsCellEmpty(Vector3Int cell)
    {
        return !occupied.ContainsKey(cell);
    }

    public void OccupyCell(Vector3Int cell)
    {
        occupied[cell] = true;
    }

    public void FreeCell(Vector3Int cell)
    {
        if (occupied.ContainsKey(cell)) occupied.Remove(cell);
    }

    public bool IsInsidePlayerZone(int playerId, Vector3Int cell)
    {
        int leftStart = 0;
        int leftEnd = gridWidth;
        int rightStart = gridWidth + Mathf.CeilToInt(centerGap / cellSize);
        int rightEnd = rightStart + gridWidth;
        bool inZ = cell.z >= 0 && cell.z < gridDepth;
        if (!inZ) return false;
        if (playerId == 1) return cell.x >= leftStart && cell.x < leftEnd;
        return cell.x >= rightStart && cell.x < rightEnd;
    }

    public bool TryGetSnappedPosition(int playerId, Vector3 worldInput, out Vector3 snapped, out Vector3Int cell)
    {
        cell = WorldToCell(worldInput);
        if (!IsInsidePlayerZone(playerId, cell)) { snapped = Vector3.zero; return false; }
        if (!IsCellEmpty(cell)) { snapped = Vector3.zero; return false; }
        snapped = CellToWorldCenter(cell);
        return true;
    }

    public bool TryGetRandomBuildCell(int playerId, out Vector3Int cell, int maxAttempts = 32)
    {
        cell = default;
        for (int i = 0; i < maxAttempts; i++)
        {
            if (playerId == 1)
            {
                int x = Random.Range(0, gridWidth);
                int z = Random.Range(0, gridDepth);
                var c = new Vector3Int(x, 0, z);
                if (IsCellEmpty(c)) { cell = c; return true; }
            }
            else
            {
                int gapCells = Mathf.CeilToInt(centerGap / cellSize);
                int start = gridWidth + gapCells;
                int x = Random.Range(start, start + gridWidth);
                int z = Random.Range(0, gridDepth);
                var c = new Vector3Int(x, 0, z);
                if (IsCellEmpty(c)) { cell = c; return true; }
            }
        }
        return false;
    }

    public Bounds GetPlayerBounds(int playerId)
    {
        float w = gridWidth * cellSize;
        float d = gridDepth * cellSize;
        float halfGap = centerGap * 0.5f;
        Vector3 size = new Vector3(w, 0.1f, d);
        Vector3 center;
        if (playerId == 1) center = new Vector3(-(w * 0.5f + halfGap), 0f, 0f);
        else center = new Vector3((w * 0.5f + halfGap), 0f, 0f);
        return new Bounds(center, size);
    }

    void OnDrawGizmos()
    {
        float w = gridWidth * cellSize;
        float d = gridDepth * cellSize;
        float halfTotal = w + centerGap;
        float halfGap = centerGap * 0.5f;

        Gizmos.color = player1Color;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 pos = new Vector3(x * cellSize - halfTotal * 0.5f + cellSize * 0.5f, 0f, z * cellSize - d * 0.5f + cellSize * 0.5f);
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.05f, cellSize));
            }
        }

        Gizmos.color = player2Color;
        int gapCells = Mathf.CeilToInt(centerGap / cellSize);
        int start = gridWidth + gapCells;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 pos = new Vector3((start + x) * cellSize - halfTotal * 0.5f + cellSize * 0.5f, 0f, z * cellSize - d * 0.5f + cellSize * 0.5f);
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.05f, cellSize));
            }
        }
    }
}
