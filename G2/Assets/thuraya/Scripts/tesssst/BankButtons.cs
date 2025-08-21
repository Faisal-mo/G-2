using UnityEngine;

public class BankButtons : MonoBehaviour
{
    public Bank bank;

    public void Add10() { if (bank) bank.AddMoney(10); }
    public void Spend10() { if (bank) bank.TrySpend(10); }
    public void ResetBank() { if (bank) bank.ResetToStart(); }
}
