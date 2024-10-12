using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenuUI : MonoBehaviour
{
    public GameObject startMenuPanel;
    public TextMeshProUGUI gameTitleText;
    public Button easyModeButton;
    public Button normalModeButton;
    public Button hardModeButton;
    public Button gameInfoButton;

    private void Start()
    {
        // Set game title
        gameTitleText.text = "Cubes Jump";

        // Add listeners to buttons
        easyModeButton.onClick.AddListener(() => StartGame(GameDifficulty.Easy));
        normalModeButton.onClick.AddListener(() => StartGame(GameDifficulty.Normal));
        hardModeButton.onClick.AddListener(() => StartGame(GameDifficulty.Hard));
        //gameInfoButton.onClick.AddListener(ShowGameInfo);

        // Show start menu
        ShowStartMenu();
    }

    private void StartGame(GameDifficulty difficulty)
    {
        GameManager.Instance.SetDifficulty(difficulty);
        GameManager.Instance.StartGame();
        HideStartMenu();
    }

    private void ShowGameInfo()
    {
        // Implement game info display logic here
        Debug.Log("Show game info");
    }

    public void ShowStartMenu()
    {
        startMenuPanel.SetActive(true);
    }

    public void HideStartMenu()
    {
        startMenuPanel.SetActive(false);
    }
}

// public enum GameDifficulty
// {
//     Easy,
//     Normal,
//     Hard
// }