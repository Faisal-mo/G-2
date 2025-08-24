using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image background;      // اسحبي Image حق الكرت هنا
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 0.84f, 0f); // ذهبي

    public void SetSelected(bool isSelected)
    {
        if (background != null)
            background.color = isSelected ? selectedColor : normalColor;
        // (اختياري) تكبير بسيط عند التحديد:
        transform.localScale = isSelected ? Vector3.one * 1.05f : Vector3.one;
    }
}
