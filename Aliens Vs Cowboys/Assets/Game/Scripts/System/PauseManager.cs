using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [Header("Input GameSettings")]
    private InputSystem_Actions inputActions; 

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;

    [Header("Buttons")]
    public Button resumeButton;
    public Button optionsButton;
    public Button mainMenuButton;

    [Header("Configuration")]
    public string mainMenuSceneName = "MainMenu";
    public bool lockCursorOnResume = true;

    public static bool IsGamePaused { get; private set; } = false;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Pause.performed += OnPausePerformed;
        inputActions.Player.Enable();
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Pause.performed -= OnPausePerformed;
        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }

    private void Start()
    {
        ResetGameState();

        if (resumeButton) resumeButton.onClick.AddListener(ResumeGame);
        if (optionsButton) optionsButton.onClick.AddListener(OpenOptions);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (optionsMenuUI != null && optionsMenuUI.activeSelf)
        {
            BackToPauseMenu();
        }
        else
        {
            if (IsGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        IsGamePaused = true;
        Time.timeScale = 0f;

        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        if (optionsMenuUI) optionsMenuUI.SetActive(false);
        inputActions.Player.Disable(); 
        inputActions.UI.Enable();      
        UnlockCursor();

        if (EventSystem.current != null && resumeButton != null)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }

        PlaySound();
    }

    public void ResumeGame()
    {
        if (AudioManager.instance != null) AudioManager.instance.SaveVolumeSettings();

        IsGamePaused = false;

        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (optionsMenuUI) optionsMenuUI.SetActive(false);

        if (lockCursorOnResume) LockCursor();
        inputActions.UI.Disable();     
        inputActions.Player.Enable();

        PlaySound();

        StopAllCoroutines();
        StartCoroutine(SmoothResume(0.5f)); 
    }

    private IEnumerator SmoothResume(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (optionsMenuUI) optionsMenuUI.SetActive(true);
        PlaySound();
    }

    public void BackToPauseMenu()
    {
        if (optionsMenuUI) optionsMenuUI.SetActive(false);
        if (pauseMenuUI) pauseMenuUI.SetActive(true);

        if (EventSystem.current != null && resumeButton != null)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
        PlaySound();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        inputActions.Player.Disable();
        inputActions.UI.Enable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlaySound();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void ResetGameState()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (optionsMenuUI) optionsMenuUI.SetActive(false);
        if (lockCursorOnResume) LockCursor();
        else UnlockCursor();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PlaySound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonClick();
        }
    }
}