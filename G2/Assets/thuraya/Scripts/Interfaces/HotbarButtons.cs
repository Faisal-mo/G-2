using UnityEngine;

public class HotbarButtons : MonoBehaviour
{
    public TowerHotbarUI ui;

    public void P1_Select(int idx) { if (ui) ui.SetP1Selected(idx); }
    public void P1_Clear() { if (ui) ui.ClearP1(); }

    public void P2_Select(int idx) { if (ui) ui.SetP2Selected(idx); }
    public void P2_Clear() { if (ui) ui.ClearP2(); }
}
