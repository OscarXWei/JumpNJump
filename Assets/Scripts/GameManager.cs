using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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
    public RawImage screenshotDisplay;
    public AudioClip backgroundMusic;
    private AudioSource audioSource;

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

        // 设置音乐
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; // 设置为循环播放
        audioSource.Play();

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

    public void SaveGame()
    {
        // save game state
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SaveGame();
            Debug.Log("Game saved successfully.");
        }
        else
        {
            Debug.LogError("SaveLoadManager instance not found!");
        }

        // capture screenshot and display it
        StartCoroutine(CaptureScreenshot());
    }
    public void LoadGame()
    {
        if (SaveLoadManager.Instance != null)
        {
            playerController.ResetGameState();
            ScoreManager.Instance.ResetScore();
            SaveLoadManager.Instance.LoadGame();

            // 加载并显示截图
            LoadSavedScreenshot();

            isStarting = true;
            isGameOver = false;
            if (gameOverUI != null) gameOverUI.HideGameOver();
            if (jumpPowerUI != null) jumpPowerUI.gameObject.SetActive(true);
            Debug.Log("Game loaded successfully.");
        }
        else
        {
            Debug.LogError("SaveLoadManager instance not found!");
        }
    }

    private void LoadSavedScreenshot()
    {
        string screenshotPath = Path.Combine(Application.persistentDataPath, "screenshot.png");
        if (File.Exists(screenshotPath))
        {
            byte[] fileData = File.ReadAllBytes(screenshotPath);
            Texture2D screenshot = new Texture2D(2, 2);
            screenshot.LoadImage(fileData);

            if (screenshotDisplay != null)
            {
                // TextMeshProUGUI imageText = screenshotDisplay.GetComponentInChildren<TextMeshProUGUI>();
                // imageText.text = "";

                screenshotDisplay.texture = screenshot;
                screenshotDisplay.gameObject.SetActive(true);
            }
            Debug.Log("Screenshot loaded successfully");
        }
        else
        {
            Debug.LogWarning("No screenshot file found!");
        }
    }

    // Coroutine to capture and save screenshot
    private IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        Camera camera = Camera.main;
        if (camera != null)
        {
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            camera.targetTexture = renderTexture;

            RenderTexture.active = renderTexture;
            camera.Render();

            // 创建Texture2D来保存截图
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);

            ShowScreenshot(screenshot);

            //保存截图到文件
            byte[] bytes = screenshot.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/screenshot.png", bytes);

            Debug.Log("Screenshot captured and displayed.");
        }
        else
        {
            Debug.LogError("Main camera not found!");
        }
    }

    private void ShowScreenshot(Texture2D screenshot)
    {
        if (screenshotDisplay != null)
        {
            screenshotDisplay.texture = screenshot; // 将截图设置为RawImage的纹理,RawImage是memory window的screensho component
            screenshotDisplay.gameObject.SetActive(true); // 确保RawImage可见
        }
        else
        {
            Debug.LogError("Screenshot display UI element not assigned!");
        }
    }



    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard
    }
}
