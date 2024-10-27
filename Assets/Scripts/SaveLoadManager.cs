using UnityEngine;
using System.IO;
using System;

[Serializable]
public class GameState
{
    public int currentScore;
    public int currentLevelIndex;
    public Vector3 playerPosition;

    public int playerHealth;

}

public class SaveLoadManager : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "gamesave.json";

    public static SaveLoadManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        GameState gameState = new GameState
        {
            currentScore = ScoreManager.Instance.GetCurrentScore(),
            currentLevelIndex = FindObjectOfType<LevelDisplayManager>().GetCurrentLevelIndex(),
            playerPosition = FindObjectOfType<PlayerController>().transform.position,
            playerHealth = FindObjectOfType<PlayerController>().getCurrentHealth()
            // 添加其他需要保存的游戏状态
        };

        string json = JsonUtility.ToJson(gameState);
        File.WriteAllText(GetSaveFilePath(), json);
        Debug.Log("Game saved successfully.");
    }

    public void LoadGame()
    {
        string filePath = GetSaveFilePath();
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameState gameState = JsonUtility.FromJson<GameState>(json);

            // 应用加载的游戏状态
            ScoreManager.Instance.SetScore(gameState.currentScore);
            FindObjectOfType<LevelDisplayManager>().SetCurrentLevelIndex(gameState.currentLevelIndex);
            FindObjectOfType<PlayerController>().transform.position = gameState.playerPosition;
            FindObjectOfType<PlayerController>().SetHealth(gameState.playerHealth);
            // 应用其他加载的游戏状态

            Debug.Log("Game loaded successfully.");
        }
        else
        {
            Debug.Log("No save file found.");
        }
    }

    private string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    }
}
