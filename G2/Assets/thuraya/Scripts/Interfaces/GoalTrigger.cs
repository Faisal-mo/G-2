using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoalTrigger : MonoBehaviour
{
    public PlayerHealth owner;

    void Awake()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<EnemyNavAI>();
        if (enemy == null) return;
        if (owner != null) owner.LoseOne();
        Destroy(enemy.gameObject);
    }
}
