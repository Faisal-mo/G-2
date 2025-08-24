using UnityEngine;
using UnityEngine.UI;

public class TowerHotbarUI : MonoBehaviour
{
    [Header("Cards (Images)")]
    public Image[] p1Cards; 
    public Image[] p2Cards;

    [Header("Sprites (Normal / Selected)")]
    public Sprite p1Normal;
    public Sprite p1Selected;
    public Sprite p2Normal;
    public Sprite p2Selected;



    
    public GameObject[] p1Highlights; 
    public GameObject[] p2Highlights;

    int p1Idx = -1;
    int p2Idx = -1;

    public void SetP1Selected(int idx) { p1Idx = idx; RefreshP1(); }
    public void SetP2Selected(int idx) { p2Idx = idx; RefreshP2(); }

    public void ClearP1() { p1Idx = -1; RefreshP1(); }
    public void ClearP2() { p2Idx = -1; RefreshP2(); }

    void RefreshP1()
    {
        if (p1Cards != null)
            for (int i = 0; i < p1Cards.Length; i++)
                if (p1Cards[i]) p1Cards[i].sprite = (i == p1Idx && p1Selected) ? p1Selected : p1Normal;

        if (p1Highlights != null)
            for (int i = 0; i < p1Highlights.Length; i++)
                if (p1Highlights[i]) p1Highlights[i].SetActive(i == p1Idx && p1Idx >= 0);
    }

    void RefreshP2()
    {
        if (p2Cards != null)
            for (int i = 0; i < p2Cards.Length; i++)
                if (p2Cards[i]) p2Cards[i].sprite = (i == p2Idx && p2Selected) ? p2Selected : p2Normal;

        if (p2Highlights != null)
            for (int i = 0; i < p2Highlights.Length; i++)
                if (p2Highlights[i]) p2Highlights[i].SetActive(i == p2Idx && p2Idx >= 0);
    }
}
