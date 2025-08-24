using UnityEngine;

public class KeyboardCardSelector : MonoBehaviour
{
    [Header("Groups")]
    [SerializeField] private CardGroup player1Group;   // اليسار (1–2)
    [SerializeField] private CardGroup player2Group;   // اليمين (8–9)

    void Update()
    {
        // Player 1: 1 و 2 (ندعم أيضاً الكيباد)
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) player1Group?.Select(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) player1Group?.Select(1);

        // Player 2: 8 و 9
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) player2Group?.Select(0);
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) player2Group?.Select(1);

        // تأكيد اختياري: Space للاعب1 / Enter للاعب2
        if (Input.GetKeyDown(KeyCode.Space)) Confirm(player1Group, "P1");
        if (Input.GetKeyDown(KeyCode.Return)) Confirm(player2Group, "P2");

        // مسح التحديد للجميع
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            player1Group?.Clear();
            player2Group?.Clear();
        }
    }

    private void Confirm(CardGroup group, string who)
    {
        var sel = group?.GetSelected();
        if (sel == null) { Debug.Log($"[{who}] لا يوجد كرت محدد"); return; }
        Debug.Log($"[{who}] CONFIRM: {sel.gameObject.name}");
        // TODO: نفّذي هنا الإجراء الحقيقي (استدعاء سباون تاور..الخ)
    }
}
