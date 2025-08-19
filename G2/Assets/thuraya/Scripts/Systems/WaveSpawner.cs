using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public WaveConfig waveConfig;
    public Transform spawnPoint;
    public Transform goal;
    public float interWaveDelay = 6f;

    int currentWave;
    bool spawning;

    void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        if (waveConfig == null || spawnPoint == null || goal == null) yield break;
        int total = Mathf.Max(1, waveConfig.totalWaves);

        while (currentWave < total)
        {
            currentWave++;
            if (IsBossWave(currentWave)) yield return StartCoroutine(SpawnBossWave());
            else yield return StartCoroutine(SpawnNormalWave(currentWave));
            yield return new WaitUntil(() => !spawning && NoEnemiesAlive());
            if (currentWave < total) yield return new WaitForSeconds(interWaveDelay);
        }
    }

    bool IsBossWave(int index) { return (index % 3) == 0; }

    bool NoEnemiesAlive() { return FindObjectsOfType<EnemyNavAI>().Length == 0; }

    IEnumerator SpawnNormalWave(int waveIndex)
    {
        spawning = true;
        var entries = waveConfig.normalWavesTemplate;
        if (entries == null || entries.Length == 0) { spawning = false; yield break; }

        int idx = Mathf.Clamp((waveIndex - 1) % entries.Length, 0, entries.Length - 1);
        var e = entries[idx];
        if (e == null || e.enemy == null || e.enemy.prefab == null) { spawning = false; yield break; }

        int count = e.count;
        float interval = Mathf.Max(0.05f, e.spawnInterval);

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy(e.enemy, false);
            yield return new WaitForSeconds(interval);
        }

        spawning = false;
    }

    IEnumerator SpawnBossWave()
    {
        spawning = true;
        if (waveConfig.bossEnemy != null && waveConfig.bossEnemy.prefab != null)
        {
            SpawnEnemy(waveConfig.bossEnemy, true);
        }
        spawning = false;
        yield return null;
    }

    void SpawnEnemy(EnemyData data, bool isBoss)
    {
        var go = Instantiate(data.prefab, spawnPoint.position, Quaternion.identity);
        var ai = go.GetComponent<EnemyNavAI>();
        if (ai == null) return;

        float hp = data.hp * (isBoss ? waveConfig.bossHpMultiplier : 1f);
        float spd = data.moveSpeed * (isBoss ? waveConfig.bossSpeedMultiplier : 1f);
        ai.Initialize(hp, spd, data.rewardOnDeath, data.armorPercent, goal);
    }
}
