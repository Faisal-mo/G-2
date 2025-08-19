using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "TD/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public GameObject prefab;
    public float hp = 100f;
    public float moveSpeed = 2f;
    public int rewardOnDeath = 5;
    [Range(0f, 0.9f)] public float armorPercent = 0f;
}
