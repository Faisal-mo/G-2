using UnityEngine;
using System;

public class Bank : MonoBehaviour
{
    public int startMoney = 20;
    public int Money { get; private set; }
    public Action<int> OnMoneyChanged;

    void Awake()
    {
        Money = startMoney;
        OnMoneyChanged?.Invoke(Money);
    }

    public bool CanAfford(int cost) => Money >= cost;

    public bool Spend(int cost)
    {
        if (!CanAfford(cost)) return false;
        Money -= cost;
        OnMoneyChanged?.Invoke(Money);
        return true;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        Money += amount;
        OnMoneyChanged?.Invoke(Money);
    }
}
