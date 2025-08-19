using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public int playerId = 1;
    public GameObject towerPrefab;
    public LayerMask groundMask = ~0;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) TryPlace();
    }

    void TryPlace()
    {
        if (cam == null || towerPrefab == null || GridManager.Instance == null) return;

        Ray r = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(r, out var hit, 1000f, groundMask)) return;

        if (GridManager.Instance.TryGetSnappedPosition(playerId, hit.point, out var snapped, out var cell))
        {
            Instantiate(towerPrefab, snapped, Quaternion.identity);
            GridManager.Instance.OccupyCell(cell);
        }
    }
}
