using UnityEngine;
using System.Collections.Generic;

public class FTowerPlacer : MonoBehaviour
{
    [Header("Tower Prefabs")]
    public GameObject[] towerPrefabsP1; 
    public GameObject[] towerPrefabsP2; 

    [Header("Mini-Boss")]
    public GameObject miniBossPrefabP1; 
    public GameObject miniBossPrefabP2;
    public Transform spawnPointP1, spawnPointP2;

    
    private int selectedTowerIndexP1 = -1; 
    private int selectedTowerIndexP2 = -1;
    private bool isUpgradingP1 = false;
    private bool isUpgradingP2 = false;
    private bool isSendingBossP1 = false;
    private bool isSendingBossP2 = false;

    void Update()
    {
        HandlePlayer1Inputs();
        HandlePlayer2Inputs();
    }

    
    void HandlePlayer1Inputs()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedTowerIndexP1 = 0; // thuraya
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedTowerIndexP1 = 1; // thuraya
        // if (Input.GetKeyDown(KeyCode.Alpha3)) selectedTowerIndexP1 = 2; // thuraya 

        // Upgrade Mode (W)
        if (Input.GetKeyDown(KeyCode.W))
        {
            isUpgradingP1 = true;
            isSendingBossP1 = false;
            Debug.Log("P1: Select a tower to upgrade!");
        }

        // Boss Mode (S)
        if (Input.GetKeyDown(KeyCode.S))
        {
            isSendingBossP1 = true;
            isUpgradingP1 = false;
            Debug.Log("P1: Press SPACE to send boss!");
        }

        // Confirm Action (Space)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isUpgradingP1) UpgradeRandomTower(1);
            else if (isSendingBossP1) SendMiniBoss(1);
            else if (selectedTowerIndexP1 != -1) PlaceTower(1, towerPrefabsP1[selectedTowerIndexP1]);
        }
    }

    // Player 2 Inputs (8-9, O, L, Enter)
    void HandlePlayer2Inputs()
    {
        // thuraya: —»ÿ 8 Ê 9 ›ﬁÿ ( 0)
        if (Input.GetKeyDown(KeyCode.Alpha8)) selectedTowerIndexP2 = 0; // thuraya
        if (Input.GetKeyDown(KeyCode.Alpha9)) selectedTowerIndexP2 = 1; // thuraya
        // if (Input.GetKeyDown(KeyCode.Alpha0)) selectedTowerIndexP2 = 2; // thuraya („Õ–Ê›)

        // Upgrade Mode (O)
        if (Input.GetKeyDown(KeyCode.O))
        {
            isUpgradingP2 = true;
            isSendingBossP2 = false;
            Debug.Log("P2: Select a tower to upgrade!");
        }

        // Boss Mode (L)
        if (Input.GetKeyDown(KeyCode.L))
        {
            isSendingBossP2 = true;
            isUpgradingP2 = false;
            Debug.Log("P2: Press ENTER to send boss!");
        }

        // Confirm Action (Enter)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isUpgradingP2) UpgradeRandomTower(2);
            else if (isSendingBossP2) SendMiniBoss(2);
            else if (selectedTowerIndexP2 != -1) PlaceTower(2, towerPrefabsP2[selectedTowerIndexP2]);
        }
    }

    void PlaceTower(int playerID, GameObject prefab)
    {
        Vector3 pos = GridManager.Instance.GetRandomBuildPosition(playerID);
        if (pos != Vector3.zero)
        {
            Instantiate(prefab, pos, Quaternion.identity);
            GridManager.Instance.OccupyCell(pos);
            Debug.Log($"P{playerID}: Placed tower!");
        }
        ResetPlayerState(playerID);
    }

    void UpgradeRandomTower(int playerID)
    {
        // Find all towers owned by this player and upgrade one randomly
        Tower[] allTowers = FindObjectsOfType<Tower>();
        List<Tower> playerTowers = new List<Tower>();
        foreach (Tower t in allTowers)
        {
            if (t.ownerID == playerID) playerTowers.Add(t);
        }
        if (playerTowers.Count > 0)
        {
            Tower randomTower = playerTowers[Random.Range(0, playerTowers.Count)];
            randomTower.Upgrade();
            Debug.Log($"P{playerID}: Upgraded a tower!");
        }
        ResetPlayerState(playerID);
    }

    void SendMiniBoss(int playerID)
    {
        Transform spawnPoint = (playerID == 1) ? spawnPointP1 : spawnPointP2;
        GameObject prefab = (playerID == 1) ? miniBossPrefabP1 : miniBossPrefabP2;
        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"P{playerID}: Sent mini-boss!");
        ResetPlayerState(playerID);
    }

    void ResetPlayerState(int playerID)
    {
        if (playerID == 1)
        {
            selectedTowerIndexP1 = -1;
            isUpgradingP1 = false;
            isSendingBossP1 = false;
        }
        else
        {
            selectedTowerIndexP2 = -1;
            isUpgradingP2 = false;
            isSendingBossP2 = false;
        }
    }
}
