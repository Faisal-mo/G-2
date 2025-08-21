using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GoalTrigger : MonoBehaviour
{
    [Tooltip("ÕÇÍÈ åĞå ÇáŞáÚÉ")]
    public PlayerHealth playerHealth;

    [Tooltip("ßã íäŞÕ ãä ÇáŞáæÈ ÚäÏ ÏÎæá ÚÏæ æÇÍÏ")]
    public int damagePerEnemy = 1;

    
    private readonly HashSet<int> processedEnemies = new HashSet<int>();

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponentInParent<EnemyNavAI>();
        if (!enemy) return;

        int id = enemy.GetInstanceID();
        if (!processedEnemies.Add(id)) return;

      
        if (playerHealth != null)
            playerHealth.Lose(Mathf.Max(1, damagePerEnemy));

        enemy.SendMessage("NotifyReachedGoal", SendMessageOptions.DontRequireReceiver);

    
        Destroy(enemy.gameObject);
    }
}
