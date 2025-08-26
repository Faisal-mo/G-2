using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class EnemyNavAI : MonoBehaviour, IDamageable
{
    public float hp = 100f;
    [Range(0f, 0.9f)] public float armorPercent = 0f;
    public int rewardOnDeath = 10;

    public float reachedDistance = 0.25f;

    public Action<EnemyNavAI> OnDied;
    public Action<EnemyNavAI> OnReachedGoal;

    NavMeshAgent agent;
    Transform[] path;
    int idx = -1;
    Transform finalGoal;
    Bank rewardToBank;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.autoBraking = true;
    }

    public void SetPath(Transform[] waypoints)
    {
        path = waypoints;
        idx = 0;
        if (path != null && path.Length > 0)
            agent.SetDestination(path[0].position);
    }

    public void Initialize(float health, float speed, int reward, float armor)
    {
        hp = health;
        rewardOnDeath = reward;
        armorPercent = Mathf.Clamp01(armor);
        agent.speed = speed;
        if ((path == null || path.Length == 0) && finalGoal != null)
            agent.SetDestination(finalGoal.position);
    }

    public void Initialize(float health, float speed, int reward, float armor, Transform[] waypoints)
    {
        Initialize(health, speed, reward, armor);
        SetPath(waypoints);
    }

    public void Initialize(float health, float speed, int reward, float armor, Transform goal)
    {
        Initialize(health, speed, reward, armor);
        finalGoal = goal;
        if (path == null || path.Length == 0)
            agent.SetDestination(finalGoal.position);
    }

    public void SetRewardBank(Bank b) { rewardToBank = b; }

    void Update()
    {
        if (agent.pathPending) return;

        if (path != null && idx >= 0 && idx < path.Length)
        {
            if (agent.remainingDistance <= reachedDistance)
            {
                idx++;
                if (idx < path.Length) agent.SetDestination(path[idx].position);
                else if (finalGoal != null) agent.SetDestination(finalGoal.position);
            }
        }
    }

    public void NotifyReachedGoal()
    {
        OnReachedGoal?.Invoke(this);
        Destroy(gameObject);
    }

    public void TakeDamage(float amount)
    {
        float reduced = amount * (1f - armorPercent);
        hp -= reduced;
        if (hp <= 0f) Die();
    }

    void Die()
    {
        if (rewardToBank != null && rewardOnDeath > 0)
            rewardToBank.AddMoney(rewardOnDeath);
        OnDied?.Invoke(this);
        Destroy(gameObject);
    }
}
