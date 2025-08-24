using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GoalTrigger : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public int damagePerEnemy = 1;

    private readonly HashSet<int> processed = new HashSet<int>();

    void OnTriggerEnter(Collider other) { TryProcess(other); }
    void OnTriggerStay(Collider other) { TryProcess(other); }

    void TryProcess(Collider other)
    {
        var enemy = other.GetComponentInParent<EnemyNavAI>();
        if (!enemy) return;

        int id = enemy.GetInstanceID();
        if (!processed.Add(id)) return;

        if (playerHealth) playerHealth.Lose(Mathf.Max(1, damagePerEnemy));
        enemy.SendMessage("NotifyReachedGoal", SendMessageOptions.DontRequireReceiver);
        Destroy(enemy.gameObject);
    }
}
