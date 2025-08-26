using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class CardHighlighter : MonoBehaviour
{
    [Header("Outline")]
    public Sprite highlightSprite;
    public Color overlayColor = new Color(1f, 1f, 1f, 0.18f);
    public bool useOutlineIfNoSprite = true;
    public Color outlineColor = new Color(1f, 0.9f, 0.2f, 0.85f);
    public Vector2 outlineDistance = new Vector2(4f, -4f);

    GameObject highlightGO;
    Outline outline;

    void Awake()
    {
        EnsureHighlight();
    }

    void EnsureHighlight()
    {
        var t = transform.Find("Highlight");
        if (!t)
        {
            highlightGO = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)highlightGO.transform;
            rt.SetParent(transform, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var img = highlightGO.GetComponent<Image>();
            img.raycastTarget = false;
            if (highlightSprite)
            {
                img.sprite = highlightSprite;
                img.type = Image.Type.Sliced;
                img.color = Color.white;
            }
            else
            {
                img.sprite = null;
                img.color = overlayColor;
            }

            highlightGO.SetActive(false);
        }
        else
        {
            highlightGO = t.gameObject;
        }

        if (!highlightSprite && useOutlineIfNoSprite)
        {
            outline = gameObject.GetComponent<Outline>();
            if (!outline) outline = gameObject.AddComponent<Outline>();
            outline.effectColor = outlineColor;
            outline.effectDistance = outlineDistance;
            outline.enabled = false;
        }
    }

    public void SetSelected(bool on)
    {
        if (highlightGO) highlightGO.SetActive(on);
        if (outline) outline.enabled = on && !highlightSprite;
        transform.localScale = on ? new Vector3(1.05f, 1.05f, 1f) : Vector3.one;
    }
}
