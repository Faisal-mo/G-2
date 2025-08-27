using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player References")]
    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    [Header("UI References")]
    public Text winnerText;
    public GameObject gameOverPanel;
    public Text player1HealthText;
    public Text player2HealthText;
    public Button restartButton;
    public Button quitButton;

    [Header("Game State")]
    public bool isGamePaused = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Auto-find player health components with YOUR base names
        FindPlayerHealthComponents();

        // Subscribe to health events
        SubscribeToHealthEvents();

        // Setup UI buttons
        SetupButtons();

        // Update initial UI
        UpdateHealthUI();

        // Ensure game starts running
        Time.timeScale = 1f;
        isGamePaused = false;

        // Hide game over panel initially
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void FindPlayerHealthComponents()
    {
        // Find by YOUR specific base names
        if (player1Health == null)
        {
            GameObject p1Base = GameObject.Find("goalP1"); // Your P1 base name
            if (p1Base != null)
            {
                player1Health = p1Base.GetComponent<PlayerHealth>();
                Debug.Log("Found Player 1 Health: " + (player1Health != null));
            }
            else
            {
                Debug.LogWarning("goalP1 object not found!");
            }
        }

        if (player2Health == null)
        {
            GameObject p2Base = GameObject.Find("goalP2"); // Your P2 base name
            if (p2Base != null)
            {
                player2Health = p2Base.GetComponent<PlayerHealth>();
                Debug.Log("Found Player 2 Health: " + (player2Health != null));
            }
            else
            {
                Debug.LogWarning("goalP2 object not found!");
            }
        }

        // Fallback: try to find any PlayerHealth components
        if (player1Health == null || player2Health == null)
        {
            PlayerHealth[] allHealth = FindObjectsOfType<PlayerHealth>();
            if (allHealth.Length >= 2)
            {
                if (player1Health == null) player1Health = allHealth[0];
                if (player2Health == null) player2Health = allHealth[1];
                Debug.Log("Used fallback to find PlayerHealth components");
            }
        }
    }

    void SubscribeToHealthEvents()
    {
        if (player1Health != null)
        {
            player1Health.OnDefeated += () => OnPlayerDefeated(1);
            player1Health.OnLivesChanged += (lives) => UpdateHealthUI();
            Debug.Log("Subscribed to Player 1 (Blue) health events");
        }
        else
        {
            Debug.LogError("Player1Health not found! Check goalP1 object.");
        }

        if (player2Health != null)
        {
            player2Health.OnDefeated += () => OnPlayerDefeated(2);
            player2Health.OnLivesChanged += (lives) => UpdateHealthUI();
            Debug.Log("Subscribed to Player 2 (Red) health events");
        }
        else
        {
            Debug.LogError("Player2Health not found! Check goalP2 object.");
        }
    }

    void SetupButtons()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void UpdateHealthUI()
    {
        if (player1HealthText != null && player1Health != null)
            player1HealthText.text = "Blue Health: " + player1Health.Lives;

        if (player2HealthText != null && player2Health != null)
            player2HealthText.text = "Red Health: " + player2Health.Lives;
    }

    void OnPlayerDefeated(int playerID)
    {
        if (isGamePaused) return;

        isGamePaused = true;
        Time.timeScale = 0f; // Pause the game

        // Determine winner - BLUE vs RED
        string winner = "";
        if (playerID == 1)  // Player 1 (Blue) defeated
        {
            winner = "PLAYER 2 (RED) WINS!";
        }
        else if (playerID == 2)  // Player 2 (Red) defeated
        {
            winner = "PLAYER 1 (BLUE) WINS!";
        }

        // Show game over UI
        ShowGameOverUI(winner);
        Debug.Log("GAME OVER - " + winner);
    }

    void ShowGameOverUI(string winnerMessage)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (winnerText != null)
        {
            winnerText.text = winnerMessage;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();

        // For testing in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (player1Health != null)
        {
            player1Health.OnDefeated -= () => OnPlayerDefeated(1);
            player1Health.OnLivesChanged -= (lives) => UpdateHealthUI();
        }

        if (player2Health != null)
        {
            player2Health.OnDefeated -= () => OnPlayerDefeated(2);
            player2Health.OnLivesChanged -= (lives) => UpdateHealthUI();
        }

        // Remove button listeners
        if (restartButton != null)
            restartButton.onClick.RemoveAllListeners();

        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();
    }

    // Debug methods to test
    [ContextMenu("Test Blue Base Defeat (Red Wins)")]
    void TestBlueDefeat()
    {
        if (player1Health != null) player1Health.Lose(player1Health.Lives);
    }

    [ContextMenu("Test Red Base Defeat (Blue Wins)")]
    void TestRedDefeat()
    {
        if (player2Health != null) player2Health.Lose(player2Health.Lives);
    }
}