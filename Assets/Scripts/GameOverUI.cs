using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public Button tryAgainButton;

    private PlayerController playerController;

    private void Start()
    {
        // Hide the game over panel at the start
        HideGameOver();

        // Add listener to the restart button
        restartButton.onClick.AddListener(RestartGame);

        // Get reference to the PlayerController
        playerController = FindObjectOfType<PlayerController>();
    }

    public void ShowGameOver(string message = "Game Over")
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = message;
        restartButton.gameObject.SetActive(true);
    }

    public void HideGameOver()
    {
        gameOverPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    public void SetTryAgainButtonActive(bool active)
    {
        tryAgainButton.gameObject.SetActive(active);
    }


    private void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}