using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuration")]
    public string mainMenuSceneName = "MainMenu";
    public string saveFileName = "savefile.json";

    [Header("Player Data")]
    public PlayerData currentData;

    [Header("Events")]
    public UnityEvent OnGameSaved;
    public UnityEvent OnGameLoaded;
    public UnityEvent<GameObject> OnPlayerRespawned;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGame();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            RespawnPlayer(player);
        }
    }

    public void StartNewGame(string sceneToLoad = "")
    {
        currentData = new PlayerData(); // Resets health, pos, AND volume to default
        SaveGame();

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(currentData, true);
        string path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"Game Saved to: {path}");
            OnGameSaved?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                currentData = JsonUtility.FromJson<PlayerData>(json);
                OnGameLoaded?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Save file corrupted, starting new. Error: {e.Message}");
                StartNewGame();
            }
        }
        else
        {
            StartNewGame();
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        if (player == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (cc != null) cc.enabled = false;
        if (rb != null) rb.isKinematic = true;

        player.transform.position = currentData.spawnPosition;
        player.transform.rotation = Quaternion.identity;

        if (cc != null) cc.enabled = true;
        if (rb != null) rb.isKinematic = false;

        currentData.currentHealth = currentData.maxHealth;
        OnPlayerRespawned?.Invoke(player);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

[System.Serializable]
public class PlayerData
{
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public Vector3 spawnPosition = new Vector3(0, 2, 0);

    public float musicVolume = 1f;
    public float sfxVolume = 1f;
}