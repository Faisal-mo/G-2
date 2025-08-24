using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelect : MonoBehaviour
{
    [SerializeField] string gameplayScene = "Gameplay";

    public void PickDiriyah() { Pick(MapId.Diriyah); }
    public void PickHijaz() { Pick(MapId.Hijaz); }
    public void PickSouth() { Pick(MapId.Egypt); }

    void Pick(MapId id)
    {
        GameSession.SelectedMap = id;
        SceneManager.LoadScene(gameplayScene);
    }

    public void BackToMenu() => SceneManager.LoadScene("MainMenu");
}
