using UnityEngine;

public class TowerGhost : MonoBehaviour
{
    public int playerId = 1;
    public GameObject previewMeshPrefab;
    public Color validColor = new Color(0f, 1f, 0f, 0.6f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.6f);

    GameObject previewMesh;
    Renderer previewRenderer;
    Vector3Int currentCell;
    bool canPlace;

    public bool CanPlace => canPlace;
    public Vector3Int CurrentCell => currentCell;

    void Start()
    {
        if (previewMeshPrefab != null)
        {
            previewMesh = Instantiate(previewMeshPrefab, transform);
            previewRenderer = previewMesh.GetComponentInChildren<Renderer>();
            if (previewRenderer != null) previewRenderer.material = new Material(previewRenderer.sharedMaterial);
        }
        UpdateVisual();
    }

    public void SetCell(Vector3Int cell)
    {
        currentCell = cell;
        Vector3 pos = GridManager.Instance.CellToWorldCenter(cell);
        transform.position = pos;
        ValidateCell();
        UpdateVisual();
    }

    void ValidateCell()
    {
        bool inside = GridManager.Instance.IsInsidePlayerZone(playerId, currentCell);
        bool empty = GridManager.Instance.IsCellEmpty(currentCell);
        canPlace = inside && empty;
    }

    void UpdateVisual()
    {
        if (previewRenderer == null) return;
        var col = canPlace ? validColor : invalidColor;
        if (previewRenderer.material.HasProperty("_Color"))
            previewRenderer.material.color = col;
    }
}
