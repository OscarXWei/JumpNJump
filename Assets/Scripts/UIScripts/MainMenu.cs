using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void NewGame()
    {
        SceneManager.LoadSceneAsync(10);
    }
    public void GoToTutorial()
    {
        SceneManager.LoadSceneAsync(5);
    }
    public void GoToSetting()
    {
        SceneManager.LoadSceneAsync(6);
    }
    public void GoToMemory()
    {
        SceneManager.LoadSceneAsync(7);
    }
    public void QuitGame()
    {
        Application.Quit();
    }


}
