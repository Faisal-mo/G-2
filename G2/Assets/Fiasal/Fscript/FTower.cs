using UnityEngine;

public class Tower : MonoBehaviour
{
    public int ownerID; // 1 for P1, 2 for P2
    public int level = 1;

    public void Upgrade()
    {
        level++;
        Debug.Log($"Tower (P{ownerID}) upgraded to Lvl {level}!");
    }
}