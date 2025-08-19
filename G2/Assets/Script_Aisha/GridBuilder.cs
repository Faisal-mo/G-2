using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 12;          
    public int height = 8;          
    public float cellSize = 2f;     
    public Vector3 origin = new Vector3(-5f, 0.02f, -5f); 

    

    
    public Vector2Int WorldToCell(Vector3 world)
    {
        Vector3 local = world - origin;
        int cx = Mathf.RoundToInt(local.x / cellSize);
        int cz = Mathf.RoundToInt(local.z / cellSize);
        return new Vector2Int(cx, cz);
    }

   
    public Vector3 CellToWorld(Vector2Int cell)
    {
        return origin + new Vector3(cell.x * cellSize, 0f, cell.y * cellSize);
    }

    public Vector3 SnapToCell(Vector3 world)
    {
        return CellToWorld(WorldToCell(world));
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 1f, 0.9f);
        for (int x = 0; x <= width; x++)
        {
            Vector3 a = origin + new Vector3(x * cellSize, 0f, 0f);
            Vector3 b = origin + new Vector3(x * cellSize, 0f, height * cellSize);
            Gizmos.DrawLine(a, b);
        }
        for (int z = 0; z <= height; z++)
        {
            Vector3 a = origin + new Vector3(0f, 0f, z * cellSize);
            Vector3 b = origin + new Vector3(width * cellSize, 0f, z * cellSize);
            Gizmos.DrawLine(a, b);
        }
    }
}
