using UnityEngine;

public class CardGroup : MonoBehaviour
{
    private CardView[] cards;

    void Awake()
    {
        cards = GetComponentsInChildren<CardView>(true);
    }

    void Refresh()
    {
        if (cards == null || cards.Length == 0)
            cards = GetComponentsInChildren<CardView>(true);
    }

    public void SelectExclusive(CardView clicked)
    {
        Refresh();
        foreach (var c in cards) c.SetSelected(c == clicked);
    }

    public void SetInteractableAll(bool on)
    {
        Refresh();
        foreach (var c in cards) c.SetInteractable(on);
    }

    public void ClearSelection()
    {
        Refresh();
        foreach (var c in cards) c.SetSelected(false);
    }
}
