using UnityEngine;
using UnityEngine.UI;

public class PlayerHeartsUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image[] heartImages;     
    public Sprite heartFull;
    public Sprite heartEmpty;

    void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnLivesChanged += Refresh;
        Refresh(playerHealth ? playerHealth.Lives : 0);
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnLivesChanged -= Refresh;
    }

    void Refresh(int lives)
    {
        if (heartImages == null) return;
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (!heartImages[i]) continue;
            heartImages[i].sprite = (i < lives) ? heartFull : heartEmpty;
            heartImages[i].enabled = true;
        }
    }
}
