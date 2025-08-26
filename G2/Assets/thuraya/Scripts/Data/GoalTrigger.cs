using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GoalTrigger : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public int damagePerEnemy = 1;

    readonly HashSet<int> processed = new HashSet<int>();

    void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponentInParent<EnemyNavAI>();
        if (!enemy) return;

        int id = enemy.GetInstanceID();
        if (!processed.Add(id)) return;

        if (playerHealth) playerHealth.Lose(Mathf.Max(1, damagePerEnemy));
        enemy.NotifyReachedGoal();
    }
}
