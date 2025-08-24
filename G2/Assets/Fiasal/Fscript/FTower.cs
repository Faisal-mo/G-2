using UnityEngine;

public class Tower : MonoBehaviour
{
    public int ownerID;
    public int level = 1;
    public int upgradeCost = 50;

    public void Upgrade()
    {
        var reg = PlayerRegistry.Instance;
        if (reg == null) return;
        var bank = reg.GetBank(ownerID);
        if (bank == null) return;
        if (!bank.Spend(upgradeCost)) return;
        level++;
        Debug.Log($"Tower (P{ownerID}) upgraded to Lvl {level}!");
    }
}
