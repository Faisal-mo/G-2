using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;

    [Header("عرض")]
    public bool showHearts = false;      // لو تبين قلوب متكررة ❤❤❤
    public string format = "❤ {0}";      // لو العرض رقمي مثل ❤ 18

#if TMP_PRESENT
    public TMP_Text tmpText;
#endif
    public Text uiText;

    void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnLivesChanged += OnLivesChanged;

        // تحديث أولي
        OnLivesChanged(playerHealth ? playerHealth.Lives : 0);
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnLivesChanged -= OnLivesChanged;
    }

    void OnLivesChanged(int lives)
    {
        string s;
        if (showHearts)
        {
            int n = Mathf.Clamp(lives, 0, 50);
            s = n > 0 ? new string('❤', n) : "❤ 0";
        }
        else
        {
            s = string.Format(format, lives);
        }

#if TMP_PRESENT
        if (tmpText) { tmpText.text = s; return; }
#endif
        if (uiText) uiText.text = s;
    }
}
