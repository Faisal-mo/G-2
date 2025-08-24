using UnityEngine;

public class CardGroup : MonoBehaviour
{
    [SerializeField] private CardView[] cards;
    public int CurrentIndex { get; private set; } = -1;

    public void Select(int index)
    {
        if (cards == null || cards.Length == 0) return;
        if (index < 0 || index >= cards.Length) return;

        if (CurrentIndex >= 0 && CurrentIndex < cards.Length)
            cards[CurrentIndex].SetSelected(false);

        CurrentIndex = index;
        cards[CurrentIndex].SetSelected(true);
        Debug.Log($"[{name}] Selected: {cards[CurrentIndex].gameObject.name}");
    }

    public void Clear()
    {
        if (CurrentIndex >= 0 && CurrentIndex < cards.Length)
            cards[CurrentIndex].SetSelected(false);
        CurrentIndex = -1;
    }

    public CardView GetSelected()
    {
        if (CurrentIndex < 0 || cards == null || CurrentIndex >= cards.Length) return null;
        return cards[CurrentIndex];
    }
}
