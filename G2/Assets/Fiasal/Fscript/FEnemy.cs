using UnityEngine;

public class FEnemy : MonoBehaviour
{
    private EnemyNavAI enemyAI;
    private float originalSpeed;
    private float slowTimer = 0f;

    void Start()
    {
        enemyAI = GetComponent<EnemyNavAI>();
        if (enemyAI != null)
        {
            // Store original speed for slow effect
            originalSpeed = enemyAI.GetComponent<UnityEngine.AI.NavMeshAgent>().speed;
        }
    }

    void Update()
    {
        // Handle slow timer
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
            {
                ResetSpeed();
            }
        }
    }

    // Compatibility method for your towers
    public void TakeDamage(float damage)
    {
        if (enemyAI != null)
            enemyAI.TakeDamage(damage);
    }

    // FIXED: Working slow method
    public void ApplySlow(float slowPercent, float duration)
    {
        if (enemyAI == null) return;

        UnityEngine.AI.NavMeshAgent agent = enemyAI.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            // Calculate slowed speed
            float slowedSpeed = originalSpeed * (1f - slowPercent / 100f);
            agent.speed = slowedSpeed;

            // Set slow duration
            slowTimer = duration;

            Debug.Log($"Enemy slowed! Speed: {agent.speed} (was {originalSpeed})");
        }
    }

    void ResetSpeed()
    {
        if (enemyAI != null)
        {
            UnityEngine.AI.NavMeshAgent agent = enemyAI.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = originalSpeed;
            }
        }
    }
}