using UnityEngine;

public class WaveHotkeys : MonoBehaviour
{
    public WaveSpawner spawner;

    void Update()
    {
        if (spawner == null) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) spawner.SendMessage("SpawnNormalWave", 1, SendMessageOptions.DontRequireReceiver);
        if (Input.GetKeyDown(KeyCode.Alpha2)) spawner.SendMessage("SpawnNormalWave", 2, SendMessageOptions.DontRequireReceiver);
        if (Input.GetKeyDown(KeyCode.Alpha3)) spawner.SendMessage("SpawnBossWave", SendMessageOptions.DontRequireReceiver);
    }
}
