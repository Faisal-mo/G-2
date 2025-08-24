using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Min(0)] public int startLives = 3;
    public int Lives { get; private set; }

    public Action<int> OnLivesChanged;
    public Action OnDefeated;

    void Awake()
    {
        Lives = Mathf.Max(0, startLives);
        OnLivesChanged?.Invoke(Lives);
    }

    public void Lose(int amount)
    {
        if (amount <= 0 || Lives <= 0) return;
        Lives = Mathf.Max(0, Lives - amount);
        OnLivesChanged?.Invoke(Lives);
        if (Lives == 0) OnDefeated?.Invoke();
    }

    public void LoseOne() => Lose(1);

    public void ResetToStart()
    {
        Lives = Mathf.Max(0, startLives);
        OnLivesChanged?.Invoke(Lives);
    }
}
