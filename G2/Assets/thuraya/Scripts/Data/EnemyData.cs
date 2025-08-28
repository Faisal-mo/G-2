using UnityEngine;

public enum MapId { Diriyah, Hijaz, Egypt }

[CreateAssetMenu(fileName = "EnemyData", menuName = "TD/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("General")]
    public string displayName;
    public GameObject basePrefab;      
    public float baseHP = 100f;
    public float moveSpeed = 2f;
    [Range(0f, 0.9f)] public float armorPercent = 0f; 
    public int rewardOnDeath = 5;

    [Header("Boss")]
    public bool isBoss = false;
    public GameObject bossPrefab_Diriyah;
    public GameObject bossPrefab_Hijaz;
    public GameObject bossPrefab_Egypt;
    public float bossHPMultiplier = 8f;
    public float bossSpeedMultiplier = 1.1f;

    public GameObject prefab => basePrefab;  
    public float hp => baseHP;
}
