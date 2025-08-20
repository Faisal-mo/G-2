using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

public class Bank : MonoBehaviour
{
    [Header("Starting Money")]
    [Min(0)] public int startingMoney = 50;

    [Header("Events")]
    public IntEvent onBalanceChanged;   

    int _money;
    public int Money => _money;

    void Awake()
    {
        _money = Mathf.Max(0, startingMoney);
    }

    void Start()
    {
        onBalanceChanged?.Invoke(_money);
    }

    public void ResetToStart()
    {
        _money = Mathf.Max(0, startingMoney);
        onBalanceChanged?.Invoke(_money);
    }

    public void SetMoney(int amount)
    {
        _money = Mathf.Max(0, amount);
        onBalanceChanged?.Invoke(_money);
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        _money += amount;
        onBalanceChanged?.Invoke(_money);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;       
        if (_money < amount) return false;  
        _money -= amount;
        onBalanceChanged?.Invoke(_money);
        return true;
    }

    public bool CanAfford(int amount) => _money >= Mathf.Max(0, amount);

    public bool TransferTo(Bank other, int amount)
    {
        if (!other) return false;
        if (!TrySpend(amount)) return false;
        other.AddMoney(amount);
        return true;
    }
}
