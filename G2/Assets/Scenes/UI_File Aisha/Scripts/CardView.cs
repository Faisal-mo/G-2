using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    public Outline outline;
    public Vector2 outlineDistance = new Vector2(4f, 4f);
    public Color outlineColor = new Color(1f, 0.84f, 0f); // #FFD700

    private bool selected;
    public bool interactable = true;

    void Awake()
    {
        if (!outline) outline = GetComponent<Outline>();
        if (!outline) outline = gameObject.AddComponent<Outline>();

        outline.effectDistance = outlineDistance;
        outline.effectColor = outlineColor;
        outline.enabled = false;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (!interactable) return;

        var group = GetComponentInParent<CardGroup>();
        if (group != null) group.SelectExclusive(this);
        else SetSelected(!selected);
    }

    public void SetSelected(bool isSelected)
    {
        selected = isSelected;
        if (outline) outline.enabled = selected;
        transform.localScale = selected ? Vector3.one * 1.05f : Vector3.one;
    }

    public void SetInteractable(bool on)
    {
        interactable = on;

        var img = GetComponent<Image>();
        if (img)
        {
            var c = img.color;
            img.color = new Color(c.r, c.g, c.b, on ? 1f : 0.6f);
        }
    }
}
