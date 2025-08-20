using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GameEnder ender;

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<EnemyNavAI>();
        if (enemy == null) return;

        enemy.NotifyReachedGoal();         
        if (ender != null) ender.OnEnemyReachedGoal(enemy);
        Destroy(enemy.gameObject);
    }
}
