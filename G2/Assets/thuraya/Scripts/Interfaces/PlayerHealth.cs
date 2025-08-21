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

   
    public void Lose(int amount)
    {
        int dmg = Mathf.Max(0, amount);
        if (dmg == 0) return;

        Lives = Mathf.Max(0, Lives - dmg);
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
