using UnityEngine;
using System.Collections.Generic;

public class BuildCursor : MonoBehaviour
{
    public int playerId = 1;
    public GameObject towerPrefab;
    public GameObject ghostPrefab;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode placeKey = KeyCode.F;
    public KeyCode removeKey = KeyCode.G;

    TowerGhost ghost;
    Vector3Int cell;
    Dictionary<Vector3Int, GameObject> placed = new Dictionary<Vector3Int, GameObject>();
    int minX;
    int maxX;
    int minZ;
    int maxZ;

    void Start()
    {
        var bounds = GridManager.Instance.GetPlayerBounds(playerId);
        var min = GridManager.Instance.WorldToCell(bounds.center - bounds.extents);
        var max = GridManager.Instance.WorldToCell(bounds.center + bounds.extents);
        minX = Mathf.Min(min.x, max.x);
        maxX = Mathf.Max(min.x, max.x);
        minZ = Mathf.Min(min.z, max.z);
        maxZ = Mathf.Max(min.z, max.z);

        int centerX = Mathf.Clamp((minX + maxX) / 2, minX, maxX);
        int centerZ = Mathf.Clamp((minZ + maxZ) / 2, minZ, maxZ);
        cell = new Vector3Int(centerX, 0, centerZ);

        var go = Instantiate(ghostPrefab);
        ghost = go.GetComponent<TowerGhost>();
        ghost.playerId = playerId;
        ghost.SetCell(cell);
    }

    void Update()
    {
        bool moved = false;
        if (Input.GetKeyDown(upKey)) { cell.z = Mathf.Clamp(cell.z + 1, minZ, maxZ); moved = true; }
        if (Input.GetKeyDown(downKey)) { cell.z = Mathf.Clamp(cell.z - 1, minZ, maxZ); moved = true; }
        if (Input.GetKeyDown(rightKey)) { cell.x = Mathf.Clamp(cell.x + 1, minX, maxX); moved = true; }
        if (Input.GetKeyDown(leftKey)) { cell.x = Mathf.Clamp(cell.x - 1, minX, maxX); moved = true; }
        if (moved) ghost.SetCell(cell);

        if (Input.GetKeyDown(placeKey)) TryPlace();
        if (Input.GetKeyDown(removeKey)) TryRemove();
    }

    void TryPlace()
    {
        if (!ghost.CanPlace) return;
        Vector3 pos = GridManager.Instance.CellToWorldCenter(cell);
        var obj = Instantiate(towerPrefab, pos, Quaternion.identity);
        placed[cell] = obj;
        GridManager.Instance.OccupyCell(cell);
        ghost.SetCell(cell);
    }

    void TryRemove()
    {
        if (!placed.ContainsKey(cell)) return;
        var obj = placed[cell];
        if (obj != null) Destroy(obj);
        placed.Remove(cell);
        GridManager.Instance.FreeCell(cell);
        ghost.SetCell(cell);
    }
}
