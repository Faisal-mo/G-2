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
    public Transform rotationPart; // Optional: assign for better visuals

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
    private Vector3 originalPosition;
    private Transform actualRotationPart; // Actual transform used for rotation

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

        // Store original position and rotation
        originalPosition = transform.position;
        defaultRotation = transform.rotation;

        // Handle rotation part assignment
        if (rotationPart != null)
        {
            actualRotationPart = rotationPart;
        }
        else
        {
            // Auto-create rotation part if not assigned
            GameObject rotPart = new GameObject("RotationPart");
            rotPart.transform.SetParent(transform);
            rotPart.transform.localPosition = Vector3.zero;
            rotPart.transform.localRotation = Quaternion.identity;
            actualRotationPart = rotPart.transform;

            // Make firePoint a child of rotation part if it exists
            if (firePoint != null)
            {
                firePoint.SetParent(actualRotationPart, true);
            }
        }

        // Setup audio source
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = volume;
            audioSource.spatialBlend = 1f;
        }

        // Freeze position with Rigidbody if exists
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
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
        // Lock position to grid - CRITICAL FIX
        transform.position = originalPosition;

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
            // Calculate direction to target (Y-axis only for horizontal rotation)
            Vector3 direction = (currentTarget.position - actualRotationPart.position).normalized;
            direction.y = 0; // Keep rotation horizontal only

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                actualRotationPart.rotation = Quaternion.Slerp(actualRotationPart.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else if (rotateToTarget)
        {
            // Smoothly return to default rotation
            actualRotationPart.rotation = Quaternion.Slerp(actualRotationPart.rotation, defaultRotation, rotationSpeed * Time.deltaTime);
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
        Vector3 directionToTarget = (target.position - actualRotationPart.position).normalized;
        float angle = Vector3.Angle(actualRotationPart.forward, directionToTarget);
        return angle < 45f; // Allow 45 degree shooting cone
    }

    void ShootProjectile(Transform target)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * projectileSpeed;
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
        Gizmos.DrawRay(actualRotationPart != null ? actualRotationPart.position : transform.position,
                      actualRotationPart != null ? actualRotationPart.forward : transform.forward * 2f);

        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(actualRotationPart != null ? actualRotationPart.position : transform.position,
                           currentTarget.position);
        }
    }

    void LateUpdate()
    {
        transform.position = originalPosition;
    }
}