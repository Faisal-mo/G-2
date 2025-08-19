using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class EnemyNavAI : MonoBehaviour, IDamageable
{
    public float hp = 100f;
    public float armorPercent = 0f;
    public int rewardOnDeath = 5;
    public Transform goal;
    public float reachedDistance = 0.35f;

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = reachedDistance;
    }

    public void Initialize(float health, float speed, int reward, float armor, Transform goalTarget)
    {
        hp = health;
        rewardOnDeath = reward;
        armorPercent = Mathf.Clamp01(armor);
        goal = goalTarget;
        agent.speed = speed;
        if (goal != null) agent.SetDestination(goal.position);
    }

    void Update()
    {
        if (goal == null) return;
        if (!agent.hasPath) agent.SetDestination(goal.position);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        float reduced = amount * (1f - armorPercent);
        hp -= reduced;
        if (hp <= 0f) Die();
    }

    void Die()
    {
        var bank = FindObjectOfType<Bank>();
        if (bank != null) bank.AddMoney(rewardOnDeath);
        Destroy(gameObject);
    }
}
