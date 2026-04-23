using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Configuration")]
    public string firstLevelName = "Level 1";

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
        if (AudioManager.instance != null) AudioManager.instance.PlayButtonClick();

        if (GameManager.Instance != null)
            GameManager.Instance.StartNewGame(firstLevelName);
        else
            SceneManager.LoadScene(firstLevelName);
    }

    public void OnContinueClicked()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayButtonClick();

        if (GameManager.Instance != null)
            GameManager.Instance.LoadGame();

        SceneManager.LoadScene(firstLevelName);
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