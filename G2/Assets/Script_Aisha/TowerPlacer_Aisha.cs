using UnityEngine;

public class TowerPlacer_Aisha : MonoBehaviour
{
    public Camera cam;
    public LayerMask groundMask;   
    public GridBuilder grid;     
    public Transform preview;      
    public GameObject towerPrefab; 
    void Update()
    {
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, 500f, groundMask))
        { if (preview) preview.gameObject.SetActive(false); return; }

        if (preview && !preview.gameObject.activeSelf) preview.gameObject.SetActive(true);

        Vector3 snap = grid.SnapToCell(hit.point);
        snap.y = hit.point.y;            
        if (preview) preview.position = snap;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            Instantiate(towerPrefab, snap, Quaternion.identity);
    }
}
