using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadSceneAsync(2);
    }
    public void GoToLevelTwo()
    {
        SceneManager.LoadSceneAsync(3);
    }
    public void GoToLevelThree()
    {
        SceneManager.LoadSceneAsync(4);
    }
    public void GoToLevelFour()
    {
        SceneManager.LoadSceneAsync(5);
    }
    public void GoToTutorial()
    {
        SceneManager.LoadSceneAsync(6);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
