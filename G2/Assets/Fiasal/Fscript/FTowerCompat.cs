using UnityEngine;
using System.Collections.Generic;

public class TowerCombat : MonoBehaviour
{
    [Header("Tower Type")]
    public TowerType towerType = TowerType.Cannon;
    public enum TowerType { Cannon, Slower, Trap }

    [Header("Combat Settings")]
    public float attackRange = 5f;
    public float attackRate = 1f;
    public int maxTargets = 2;
    public LayerMask enemyLayer;

    [Header("Damage/Slow Values")]
    public float damage = 10f;
    public float slowPercent = 30f;
    public float slowDuration = 3f;

    [Header("Cannon Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 15f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public bool rotateToTarget = true;

    [Header("Trap Effects")]
    public ParticleSystem trapEffect;
    public Light trapLight;

    [Header("Slower Effects")]
    public ParticleSystem slowEffect;
    public AudioClip slowApplySound;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip cannonShotSound;
    public AudioClip trapActiveSound;
    public AudioClip trapDamageSound;
    [Range(0f, 1f)] public float volume = 0.7f;

    private float attackTimer;
    private List<FEnemy> targets = new List<FEnemy>();
    private int myOwnerID;
    private Transform currentTarget;
    private Quaternion defaultRotation;

    void Start()
    {
        // Get ownerID from the Tower component
        Tower tower = GetComponent<Tower>();
        if (tower != null)
        {
            myOwnerID = tower.ownerID;
        }
        else
        {
            Debug.LogError("No Tower component found! Add Tower.cs to all towers.");
        }

        // Store default rotation
        defaultRotation = transform.rotation;

        // Setup audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = volume;
            audioSource.spatialBlend = 1f;
        }

        // Enable tower-specific visuals
        switch (towerType)
        {
            case TowerType.Trap:
                if (trapEffect != null) trapEffect.Play();
                if (trapLight != null) trapLight.enabled = true;

                if (trapActiveSound != null)
                {
                    audioSource.clip = trapActiveSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
                break;

            case TowerType.Slower:
                if (slowEffect != null) slowEffect.Play();
                break;
        }
    }

    void Update()
    {
        switch (towerType)
        {
            case TowerType.Cannon:
                HandleCannonRotation();
                HandleTargetedAttack();
                break;

            case TowerType.Slower:
                HandleTargetedAttack();
                break;

            case TowerType.Trap:
                HandleTrapDamage();
                break;
        }
    }

    void HandleCannonRotation()
    {
        if (rotateToTarget && currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (rotateToTarget)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleTargetedAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackRate)
        {
            FindTargets();
            if (targets.Count > 0)
            {
                AttackTargets();
                attackTimer = 0f;
            }
        }
    }

    void HandleTrapDamage()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider col in enemiesInRange)
        {
            FEnemy enemy = col.GetComponent<FEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage * Time.deltaTime);
                PlayTrapDamageSound();
                ShowDamageEffect(enemy.transform);
            }
        }
    }

    void FindTargets()
    {
        targets.Clear();
        currentTarget = null;

        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        List<FEnemy> validEnemies = new List<FEnemy>();
        foreach (Collider col in enemiesInRange)
        {
            FEnemy enemy = col.GetComponent<FEnemy>();
            if (enemy != null)
                validEnemies.Add(enemy);
        }

        validEnemies.Sort((a, b) =>
            Vector3.Distance(a.transform.position, transform.position)
            .CompareTo(Vector3.Distance(b.transform.position, transform.position))
        );

        int targetCount = (towerType == TowerType.Slower) ? maxTargets : 1;
        for (int i = 0; i < Mathf.Min(targetCount, validEnemies.Count); i++)
        {
            targets.Add(validEnemies[i]);
        }

        if (targets.Count > 0)
        {
            currentTarget = targets[0].transform;
        }
    }

    void AttackTargets()
    {
        foreach (FEnemy target in targets)
        {
            if (target == null) continue;

            switch (towerType)
            {
                case TowerType.Cannon:
                    if (IsFacingTarget(target.transform))
                    {
                        PlayCannonSound();
                        ShootProjectile(target.transform);
                        target.TakeDamage(damage);
                        Debug.Log($"P{myOwnerID} Cannon hit enemy for {damage} damage!");
                    }
                    break;

                case TowerType.Slower:
                    PlaySlowSound();
                    target.ApplySlow(slowPercent, slowDuration);
                    Debug.Log($"P{myOwnerID} Slower slowed enemy by {slowPercent}%!");
                    ShowSlowEffect(target.transform);
                    break;
            }
        }
    }

    bool IsFacingTarget(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        return angle < 30f;
    }

    void ShootProjectile(Transform target)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector3 direction = (target.position - firePoint.position).normalized;
        projectile.transform.rotation = Quaternion.LookRotation(direction);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }

        Destroy(projectile, 2f);
    }

    void PlayCannonSound()
    {
        if (cannonShotSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(cannonShotSound);
        }
    }

    void PlaySlowSound()
    {
        if (slowApplySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(slowApplySound);
        }
    }

    float lastDamageSoundTime = 0f;
    void PlayTrapDamageSound()
    {
        if (trapDamageSound != null && audioSource != null &&
            Time.time - lastDamageSoundTime > 0.3f)
        {
            audioSource.PlayOneShot(trapDamageSound);
            lastDamageSoundTime = Time.time;
        }
    }

    void ShowDamageEffect(Transform enemy)
    {
        GameObject hitEffect = new GameObject("TrapHitEffect");
        hitEffect.transform.position = enemy.position + Vector3.up;
        Destroy(hitEffect, 0.3f);
    }

    void ShowSlowEffect(Transform enemy)
    {
        GameObject slowEffect = new GameObject("SlowEffect");
        slowEffect.transform.position = enemy.position;
        slowEffect.transform.SetParent(enemy);
        Destroy(slowEffect, slowDuration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = (towerType == TowerType.Trap) ? Color.yellow :
                      (towerType == TowerType.Slower) ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}