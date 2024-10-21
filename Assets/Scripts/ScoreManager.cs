using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TMP_Text scoreText;
    private int currentScore = 0;
    private int lastScore = 0;

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

    private void Start()
    {
        UpdateScoreDisplay();
    }

    public void AddScore(int points)
    {
        lastScore = currentScore;
        currentScore += points;
        UpdateScoreDisplay();
    }

    public void ResetScore()
    {
        //lastScore = currentScore;
        currentScore = 0;
        UpdateScoreDisplay();
    }

    public void RetryScore()
    {
        //currentScore = lastScore;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void SetScore(int score)
    {
        currentScore = score;
        UpdateScoreDisplay();
    }

}
