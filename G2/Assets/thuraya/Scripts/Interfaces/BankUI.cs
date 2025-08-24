using UnityEngine;
using TMPro;

public class BankUI : MonoBehaviour
{
    public Bank bank;
    public TMP_Text tmpText;

    void OnEnable()
    {
        if (bank) bank.onBalanceChanged.AddListener(OnBalanceChanged);
        OnBalanceChanged(bank ? bank.Money : 0);
    }

    void OnDisable()
    {
        if (bank) bank.onBalanceChanged.RemoveListener(OnBalanceChanged);
    }

    void OnBalanceChanged(int amount)
    {
        if (tmpText) tmpText.text = amount.ToString();
    }
}
