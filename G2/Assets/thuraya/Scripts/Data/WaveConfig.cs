using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "TD/Wave Config")]
public class WaveConfig : ScriptableObject
{
    public int totalWaves = 9;

    [System.Serializable]
    public class WaveEntry
    {
        public EnemyData enemy;
        public int count = 3;
        public float spawnInterval = 0.8f;
    }

    public WaveEntry[] normalWavesTemplate;
    public EnemyData bossEnemy;
    public float bossHpMultiplier = 6f;
    public float bossSpeedMultiplier = 1.1f;
}
