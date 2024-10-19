using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerController playerController;
    //public PlatformManager platformManager;
    public GameOverUI gameOverUI;
    public JumpPowerUI jumpPowerUI;
    public StartMenuUI startMenuUI;
    public bool isStarting = false;
    public bool isGameOver { get; private set; } = false;
    public bool IsEasyMode { get; private set; } = false;

    private GameDifficulty currentDifficulty = GameDifficulty.Easy;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Find references if not set in inspector
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        // if (platformManager == null)
        //     platformManager = FindObjectOfType<PlatformManager>();
        if (gameOverUI == null)
            gameOverUI = FindObjectOfType<GameOverUI>();
        if (jumpPowerUI == null)
            jumpPowerUI = FindObjectOfType<JumpPowerUI>();
        if (startMenuUI == null)
            startMenuUI = FindObjectOfType<StartMenuUI>();

        // Show start menu
        ShowStartMenu();
    }

    public void ShowStartMenu()
    {
        // Reset game state
        isStarting = false;
        isGameOver = false;

        // Hide all UI elements except start menu
        if (gameOverUI != null)
            gameOverUI.HideGameOver();
        if (jumpPowerUI != null)
            jumpPowerUI.gameObject.SetActive(false);

        // Show start menu
        if (startMenuUI != null)
            startMenuUI.ShowStartMenu();
    }

    public void SetDifficulty(GameDifficulty difficulty)
    {
        currentDifficulty = difficulty;
    }

    public void StartGame()
    {
        // Apply difficulty settings
        // ApplyDifficultySettings();
        startMenuUI.HideStartMenu();
        gameOverUI.HideGameOver();
        isGameOver = false;
        isStarting = true;

        // // Reset platforms
        // if (platformManager != null)
        //     platformManager.ResetPlatforms();

        // Reset player
        if (playerController != null)
            playerController.ResetGameState();

        // Reset score
        ScoreManager.Instance.ResetScore();

        Debug.Log($"Game Started with {currentDifficulty} difficulty");
    }

    public void RestartGame()
    {
        // Hide game over UI
        if (gameOverUI != null)
            gameOverUI.HideGameOver();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver(string message = "Game Over")
    {
        isGameOver = true;
        isStarting = false;
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver(message);
        }
    }

    private void ApplyDifficultySettings()
    {
        switch (currentDifficulty)
        {
            case GameDifficulty.Easy:
                SetEasyMode();
                break;
            case GameDifficulty.Normal:
                SetNormalMode();
                break;
            case GameDifficulty.Hard:
                SetHardMode();
                break;
        }

        IsEasyMode = currentDifficulty == GameDifficulty.Easy;
    }

    private void SetEasyMode()
    {
        if (jumpPowerUI != null)
            jumpPowerUI.gameObject.SetActive(true);
        if (gameOverUI != null)
            gameOverUI.SetTryAgainButtonActive(true);
        // if (platformManager != null)
        //     platformManager.SetPlatformScale(Vector3.one);
    }

    private void SetNormalMode()
    {
        if (jumpPowerUI != null)
            jumpPowerUI.gameObject.SetActive(false);
        if (gameOverUI != null)
            gameOverUI.SetTryAgainButtonActive(false);
        // if (platformManager != null)
        //     platformManager.SetPlatformScale(Vector3.one);
    }

    private void SetHardMode()
    {
        if (jumpPowerUI != null)
            jumpPowerUI.gameObject.SetActive(false);
        if (gameOverUI != null)
            gameOverUI.SetTryAgainButtonActive(false);
        // if (platformManager != null)
        //     platformManager.SetPlatformScale(new Vector3(0.5f, 0.5f, 0.5f));
    }
}

public enum GameDifficulty
{
    Easy,
    Normal,
    Hard
}