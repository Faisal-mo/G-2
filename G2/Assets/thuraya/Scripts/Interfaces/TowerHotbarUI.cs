using UnityEngine;
using UnityEngine.UI;

public class TowerHotbarUI : MonoBehaviour
{
    public Image[] p1Cards;
    public Image[] p2Cards;
    public Color highlightColor = new Color(1f, 1f, 0f, 0.35f);
    public float selectedScale = 1.05f;

    GameObject[] p1HL;
    GameObject[] p2HL;

    int p1Idx = -1;
    int p2Idx = -1;

    void Awake()
    {
        p1HL = BuildOverlays(p1Cards);
        p2HL = BuildOverlays(p2Cards);
    }

    GameObject[] BuildOverlays(Image[] cards)
    {
        if (cards == null) return null;
        var arr = new GameObject[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            var img = cards[i];
            if (!img) continue;
            var t = img.transform.Find("HL");
            GameObject go;
            if (t == null)
            {
                go = new GameObject("HL", typeof(RectTransform), typeof(Image));
                var rt = (RectTransform)go.transform;
                rt.SetParent(img.transform, false);
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                var over = go.GetComponent<Image>();
                over.raycastTarget = false;
                over.sprite = null;
                over.color = highlightColor;
                go.SetActive(false);
            }
            else
            {
                go = t.gameObject;
            }
            arr[i] = go;
        }
        return arr;
    }

    public void SetP1Selected(int idx) { p1Idx = idx; Refresh(p1Cards, p1HL, p1Idx); }
    public void SetP2Selected(int idx) { p2Idx = idx; Refresh(p2Cards, p2HL, p2Idx); }
    public void ClearP1() { p1Idx = -1; Refresh(p1Cards, p1HL, p1Idx); }
    public void ClearP2() { p2Idx = -1; Refresh(p2Cards, p2HL, p2Idx); }

    void OnEnable()
    {
        Refresh(p1Cards, p1HL, p1Idx);
        Refresh(p2Cards, p2HL, p2Idx);
    }

    void Refresh(Image[] cards, GameObject[] overlays, int idx)
    {
        if (cards == null) return;
        for (int i = 0; i < cards.Length; i++)
        {
            var img = cards[i];
            if (!img) continue;
            bool on = (i == idx && idx >= 0);
            if (overlays != null && i < overlays.Length && overlays[i] != null)
                overlays[i].SetActive(on);
            img.transform.localScale = on ? new Vector3(selectedScale, selectedScale, 1f) : Vector3.one;
        }
    }
}
