using System;                     
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class EnemyNavAI : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float hp = 100f;
    [Range(0f, 0.9f)] public float armorPercent = 0f;
    public int rewardOnDeath = 5;

    [Header("Navigation")]
    public float reachedDistance = 0.25f;

   
    public Action<EnemyNavAI> OnDied;
    public Action<EnemyNavAI> OnReachedGoal;

    private NavMeshAgent agent;
    private Transform[] path;
    private int idx = -1;
    private Transform finalGoal;     
    private Bank rewardToBank;       

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

    public void Initialize(float health, float speed, int reward, float armor, Bank senderBank)
    {
        Initialize(health, speed, reward, armor);
        rewardToBank = senderBank;
    }

    public void Initialize(float health, float speed, int reward, float armor, object _)
    {
        Initialize(health, speed, reward, armor);
    }

    public void InitFromData(EnemyData data, bool isBoss,
                             Transform[] waypoints, Transform goal, Bank senderBank)
    {
        float hpMult = isBoss ? data.bossHPMultiplier : 1f;
        float spdMult = isBoss ? data.bossSpeedMultiplier : 1f;

        hp = data.baseHP * hpMult;
        rewardOnDeath = data.rewardOnDeath;
        armorPercent = Mathf.Clamp01(data.armorPercent);
        agent.speed = data.moveSpeed * spdMult;

        rewardToBank = senderBank;
        finalGoal = goal;

        SetPath(waypoints);
        if ((path == null || path.Length == 0) && finalGoal != null)
            agent.SetDestination(finalGoal.position);
    }

    void Update()
    {
        if (agent.pathPending) return;

        if (path != null && idx >= 0 && idx < path.Length)
        {
            if (agent.remainingDistance <= reachedDistance)
            {
                idx++;
                if (idx < path.Length)
                {
                    agent.SetDestination(path[idx].position);
                }
                else if (finalGoal != null)
                {
                    agent.SetDestination(finalGoal.position);
                }
            }
        }
    }

    public void NotifyReachedGoal()
    {
        OnReachedGoal?.Invoke(this);
    }

    public void TakeDamage(float amount)
    {
        float reduced = amount * (1f - armorPercent);
        hp -= reduced;
        if (hp <= 0f) Die();
    }

    private void Die()
    {
        if (rewardToBank != null)
            rewardToBank.AddMoney(rewardOnDeath);
        OnDied?.Invoke(this); 
        Destroy(gameObject);
    }
}
