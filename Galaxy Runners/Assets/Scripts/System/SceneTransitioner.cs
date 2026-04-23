using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainMenuScene = "Main Menu";


    public string level1Scene = "Level 1";



    public void LoadLevel1()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame(level1Scene);
        }
        else
        {
            SceneManager.LoadScene(level1Scene);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartSpecificScene(string sceneName)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame(sceneName);
        }
    }
}