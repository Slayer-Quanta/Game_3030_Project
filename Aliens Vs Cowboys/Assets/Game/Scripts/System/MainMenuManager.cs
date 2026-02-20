using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The name of the level to load for a New Game.")]
    public string firstLevelName = "GameLevel";

    [Header("UI Panels & Prefabs")]
    public GameObject mainMenuContent;
    public GameObject optionsPrefab;

    [Header("References")]
    public SceneTransition sceneTransitioner;
    public Button continueButton;

    private GameObject optionsInstance;
    private OptionsMenu optionsMenu;

    private void Awake()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        ShowMainMenu();

        if (continueButton != null)
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "savefile.json");
            continueButton.interactable = System.IO.File.Exists(path);
        }
    }

    private void OnDestroy()
    {
        if (optionsMenu != null)
            optionsMenu.OnReturnToParentMenu -= ShowMainMenu;

        if (optionsInstance != null)
            Destroy(optionsInstance);
    }

    public void ShowMainMenu()
    {
        mainMenuContent?.SetActive(true);

        if (optionsInstance != null)
        {
            if (optionsMenu != null)
                optionsMenu.OnReturnToParentMenu -= ShowMainMenu;

            Destroy(optionsInstance);
            optionsInstance = null;
            optionsMenu = null;
        }

        if (AudioManager.instance != null) AudioManager.instance.PlayButtonClick();
    }

    public void OpenOptions()
    {
        if (optionsPrefab == null)
        {
            Debug.LogError("Options Prefab is missing from MainMenuController!");
            return;
        }

        mainMenuContent?.SetActive(false);

        if (optionsInstance == null)
        {
            optionsInstance = Instantiate(optionsPrefab);
            optionsInstance.SetActive(true);
            optionsMenu = optionsInstance.GetComponentInChildren<OptionsMenu>();

            if (optionsMenu != null)
            {
                optionsMenu.OnReturnToParentMenu += ShowMainMenu;
                optionsMenu.Initialize();
            }
        }

        if (AudioManager.instance != null) AudioManager.instance.PlayButtonClick();
    }

    public void OnNewGameClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame();
        }

        if (AudioManager.instance != null) AudioManager.instance.PlayButtonClick();

        // Use transitioner if available, otherwise fallback to direct load
        //if (sceneTransitioner != null)
        //{
        //    sceneTransitioner.LoadTutorial();
        //}
        //else
        //{
        //    SceneManager.LoadScene(firstLevelName);
        //}
    }

    public void OnContinueClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame();

            if (AudioManager.instance != null) AudioManager.instance.PlayButtonClick();

            // Load the level (Assuming New Game and Continue go to the same starting level, 
            // or modify to load a saved scene name from currentData)
            //if (sceneTransitioner != null)
            //    sceneTransitioner.LoadTutorial();
            else
                SceneManager.LoadScene(firstLevelName);
        }
    }

    public void OnQuitClicked()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayButtonClick();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else if (sceneTransitioner != null)
        {
            sceneTransitioner.QuitGame();
        }
        else
        {
            Application.Quit();
        }
    }
}