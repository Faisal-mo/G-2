using UnityEngine;
using UnityEngine.AI;

public class WaveSpawner : MonoBehaviour
{
    [Header("Config")]
    public WaveConfig waveConfig;
    public int totalWaves = 9;
    public float interWaveDelay = 4f;

    [Header("Next Wave Logic")]
    public bool waitClearForNextWave = false;
    public float clearTimeout = 20f;

    [Header("Path & Spawn")]
    public Transform spawnPoint;
    public PathWaypoints path;
    public Transform goal;

    [Header("Economy")]
    public Bank ownerBank;                // اربطيه P1/P2

    [Header("Visuals")]
    public bool tintNormalsByMap = true;

    [Header("Side / Map Override")]
    public string sideName = "P1";
    public bool overrideMapInInspector = false;
    public MapId inspectorMap = MapId.Diriyah;

    [Header("Auto Start")]
    public bool autoStartWhenReady = true;

    int currentWave;
    bool spawning;
    bool started;

    public int CurrentWave => currentWave;
    public int TotalWaves => totalWaves;
    public bool IsSpawning => spawning;

    MapId CurrentMap => overrideMapInInspector ? inspectorMap : GameSession.SelectedMap;

    void OnEnable() { TryStart(); }
    void Start() { TryStart(); }

    // يستدعى يدويًا من MapLoader بعد الربط
    public void StartWaves()
    {
        if (started) return;
        if (!IsReady()) { Debug.LogWarning($"[WaveSpawner:{sideName}] not ready to start"); return; }
        started = true;
        StartCoroutine(RunWaves());
    }

    void TryStart()
    {
        if (started || !autoStartWhenReady) return;
        if (!IsReady()) return; // ننتظر MapLoader يملأ الحقول
        started = true;
        StartCoroutine(RunWaves());
    }

    bool IsReady()
    {
        if (!waveConfig) return false;
        if (!spawnPoint || !goal) return false;
        if (!path || path.waypoints == null || path.waypoints.Length == 0) return false;
        if (waveConfig.totalWaves > 0) totalWaves = waveConfig.totalWaves;
        return true;
    }

    System.Collections.IEnumerator RunWaves()
    {
        int total = Mathf.Max(1, totalWaves);

        while (currentWave < total)
        {
            currentWave++;
            bool isBoss = (currentWave % 3) == 0;
            Debug.Log($"[WaveSpawner:{sideName}] Wave {currentWave}/{total} {(isBoss ? "(BOSS)" : "(Normal)")}");

            if (isBoss) yield return StartCoroutine(SpawnBossWave());
            else yield return StartCoroutine(SpawnNormalWave(currentWave));

            if (waitClearForNextWave)
                yield return StartCoroutine(WaitUntilClearedOrTimeout(clearTimeout));

            if (currentWave < total)
                yield return new WaitForSeconds(interWaveDelay);
        }
    }

    System.Collections.IEnumerator WaitUntilClearedOrTimeout(float maxSeconds)
    {
        float t = 0f;
        while (t < maxSeconds)
        {
            if (!spawning && NoEnemiesAlive()) yield break;
            t += Time.deltaTime;
            yield return null;
        }
        Debug.LogWarning($"[WaveSpawner:{sideName}] Clear wait timed out — moving to next wave.");
    }

    bool NoEnemiesAlive() => FindObjectsOfType<EnemyNavAI>().Length == 0;

    System.Collections.IEnumerator SpawnNormalWave(int waveIndex)
    {
        spawning = true;

        var entries = waveConfig.normalWavesTemplate;
        if (entries == null || entries.Length == 0) { spawning = false; yield break; }

        int idx = Mathf.Clamp((waveIndex - 1) % entries.Length, 0, entries.Length - 1);
        var e = entries[idx];
        if (e == null || e.enemy == null || e.enemy.prefab == null) { spawning = false; yield break; }

        int count = Mathf.Max(1, e.count);
        float interval = Mathf.Max(0.05f, e.spawnInterval);

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy(e.enemy, false);
            yield return new WaitForSeconds(interval);
        }

        spawning = false;
    }

    System.Collections.IEnumerator SpawnBossWave()
    {
        spawning = true;

        if (waveConfig.bossEnemy != null)
            SpawnEnemy(waveConfig.bossEnemy, true);

        spawning = false;
        yield return null;
    }

    void SpawnEnemy(EnemyData data, bool isBoss)
    {
        if (!data) return;

        GameObject prefabToUse = isBoss
            ? (CurrentMap == MapId.Diriyah ? (data.bossPrefab_Diriyah ? data.bossPrefab_Diriyah : data.prefab)
              : CurrentMap == MapId.Hijaz ? (data.bossPrefab_Hijaz ? data.bossPrefab_Hijaz : data.prefab)
                                            : (data.bossPrefab_Egypt ? data.bossPrefab_Egypt : data.prefab))
            : data.prefab;

        if (!prefabToUse) { Debug.LogWarning($"[WaveSpawner:{sideName}] Chosen prefab is NULL"); return; }

        var go = Instantiate(prefabToUse, spawnPoint.position, Quaternion.identity);

        var agent = go.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, 1f, NavMesh.AllAreas))
                agent.Warp(hit.position);
        }

        if (!isBoss && tintNormalsByMap) ApplyTintByMap(go, data);

        float hp = data.hp * (isBoss ? waveConfig.bossHpMultiplier : 1f);
        float spd = data.moveSpeed * (isBoss ? waveConfig.bossSpeedMultiplier : 1f);

        var ai = go.GetComponent<EnemyNavAI>();
        if (ai != null)
        {
            if (path && path.waypoints != null && path.waypoints.Length > 0)
                ai.SetPath(path.waypoints);

            ai.Initialize(hp, spd, data.rewardOnDeath, data.armorPercent, goal);

            if (ownerBank != null)
                ai.SendMessage("SetRewardBank", ownerBank, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning($"[WaveSpawner:{sideName}] Spawned prefab has no EnemyNavAI!");
        }
    }

    void ApplyTintByMap(GameObject root, EnemyData data)
    {
        Color color = (CurrentMap == MapId.Diriyah) ? data.colorDiriyah
                    : (CurrentMap == MapId.Hijaz) ? data.colorHijaz
                                                    : data.colorEgypt;

        var rends = root.GetComponentsInChildren<Renderer>(true);
        foreach (var r in rends)
        {
            if (!r) continue;
            var mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];
                if (m && m.HasProperty("_Color")) m.color = color;
            }
        }
    }
}
