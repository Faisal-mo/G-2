using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GoalTrigger : MonoBehaviour
{
    [Tooltip("GameEnder  lives/onLose)")]
    public GameEnder ender;

    [Tooltip("damigimpact")]
    public int damagePerEnemy = 1;

    private readonly HashSet<int> processedEnemies = new HashSet<int>();

    private void OnTriggerEnter(Collider other)
    {

        var enemy = other.GetComponentInParent<EnemyNavAI>();
        if (enemy == null) return;

        int id = enemy.GetInstanceID();
        if (!processedEnemies.Add(id)) return; 

        enemy.NotifyReachedGoal();

     
        if (ender != null)
            ender.ApplyDamage(Mathf.Max(1, damagePerEnemy));

        
        Destroy(enemy.gameObject);
    }
}
