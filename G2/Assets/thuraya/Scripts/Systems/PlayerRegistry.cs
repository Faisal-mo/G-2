using UnityEngine;

public class PlayerRegistry : MonoBehaviour
{
    public static PlayerRegistry Instance { get; private set; }
    public Bank bankP1;
    public Bank bankP2;

    void Awake()
    {
        Instance = this;
    }

    public Bank GetBank(int playerID)
    {
        return playerID == 1 ? bankP1 : bankP2;
    }
}
