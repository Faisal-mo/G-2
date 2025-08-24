using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMain : MonoBehaviour
{
    [SerializeField] string mapSelectScene = "MapSelect";

    public void Play() => SceneManager.LoadScene(mapSelectScene);

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
