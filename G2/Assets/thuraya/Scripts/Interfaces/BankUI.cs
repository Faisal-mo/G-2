using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

public class BankUI : MonoBehaviour
{
    [Header("Refs")]
    public Bank bank;

    [Header("Format")]
    public string prefix = "$ ";
    public string suffix = "";

#if TMP_PRESENT
    public TMP_Text tmpText;
#endif
    public Text uiText;

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
        string s = prefix + amount.ToString() + suffix;
#if TMP_PRESENT
        if (tmpText) { tmpText.text = s; return; }
#endif
        if (uiText) uiText.text = s;
    }
}
