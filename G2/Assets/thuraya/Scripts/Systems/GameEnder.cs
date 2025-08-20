using UnityEngine;
using UnityEngine.Events;

public class GameEnder : MonoBehaviour
{
    [Header("Base Health / Lives")]
    public int lives = 20;

    [Header("Events")]
    public UnityEvent onLose;
    public UnityEvent onWin;

    // ÇÓÊÏÚÇÁ ÖÑÑ ÚÇã
    public void ApplyDamage(int amount)
    {
        int dmg = Mathf.Max(0, amount);
        if (dmg == 0) return;

        lives = Mathf.Max(0, lives - dmg);
        if (lives == 0)
        {
            onLose?.Invoke();
        }
    }

    // ááÊæÇÝÞ ãÚ ÃßæÇÏ ÞÏíãÉ
    public void OnEnemyReachedGoal(Component _)
    {
        ApplyDamage(1);
    }

    public void ForceWin() => onWin?.Invoke();
}
