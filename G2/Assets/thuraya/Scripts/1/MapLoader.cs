using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapLoader : MonoBehaviour
{
    public string diriyahScene = "Level_Diriyah";
    public string hijazScene = "Level_Hijaz";
    public string southScene = "Level_South";

    public WaveSpawner spawnerP1;
    public WaveSpawner spawnerP2;
    public PlayerHealth playerP1;
    public PlayerHealth playerP2;

    void Awake()
    {
        if (spawnerP1) spawnerP1.enabled = false;
        if (spawnerP2) spawnerP2.enabled = false;
    }

    void Start() { StartCoroutine(LoadLevelAndBind()); }

    IEnumerator LoadLevelAndBind()
    {
        var op = SceneManager.LoadSceneAsync(GetLevelName(), LoadSceneMode.Additive);
        yield return op;

        var anchors = FindObjectOfType<LevelAnchors>(true);
        if (!anchors) yield break;

        BindSpawner(spawnerP1, anchors.spawnP1, anchors.pathP1, anchors.goalP1);
        BindSpawner(spawnerP2, anchors.spawnP2, anchors.pathP2, anchors.goalP2);

        BindGoalHealth(anchors.goalP1, playerP1);
        BindGoalHealth(anchors.goalP2, playerP2);

        if (spawnerP1) { spawnerP1.enabled = true; spawnerP1.StartWaves(); }
        if (spawnerP2) { spawnerP2.enabled = true; spawnerP2.StartWaves(); }
    }

    void BindSpawner(WaveSpawner sp, Transform spw, PathWaypoints path, Transform goal)
    {
        if (!sp) return;
        sp.spawnPoint = spw;
        sp.path = path;
        sp.goal = goal;
        sp.autoStartWhenReady = false;
    }

    void BindGoalHealth(Transform goal, PlayerHealth ph)
    {
        if (!goal) return;
        var gt = goal.GetComponentInChildren<GoalTrigger>(true);
        if (gt) gt.playerHealth = ph;
    }

    string GetLevelName()
    {
        switch (GameSession.SelectedMap)
        {
            case MapId.Diriyah: return diriyahScene;
            case MapId.Hijaz: return hijazScene;
            default: return southScene;
        }
    }
}
