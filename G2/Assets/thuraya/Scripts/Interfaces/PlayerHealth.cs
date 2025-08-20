using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int startLives = 20;
    public int Lives { get; private set; }
    public Action<int> OnLivesChanged;
    public Action OnDefeated;

    void Awake()
    {
        Lives = startLives;
        OnLivesChanged?.Invoke(Lives);
    }

    public void LoseOne()
    {
        Lives = Mathf.Max(0, Lives - 1);
        OnLivesChanged?.Invoke(Lives);
        if (Lives == 0) OnDefeated?.Invoke();
    }
}
