using UnityEngine;
using UnityEngine.Events;

public class Bank : MonoBehaviour
{
    public int startMoney = 100;
    public UnityEvent<int> onBalanceChanged = new UnityEvent<int>();

    int _money;
    public int Money => _money;

    void Awake()
    {
        _money = startMoney;
        onBalanceChanged.Invoke(_money);
    }

    public bool CanAfford(int amount) => _money >= amount;

    public bool Spend(int amount)
    {
        if (_money < amount) return false;
        _money -= amount;
        onBalanceChanged.Invoke(_money);
        return true;
    }

    public bool TrySpend(int amount) => Spend(amount);

    public void Deposit(int amount)
    {
        _money += amount;
        onBalanceChanged.Invoke(_money);
    }

    public void AddMoney(int amount) => Deposit(amount);

    public void ResetToStart()
    {
        _money = startMoney;
        onBalanceChanged.Invoke(_money);
    }
}
