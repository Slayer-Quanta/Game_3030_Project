using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Data")]
    public PlayerData currentData;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            RespawnPlayer(player);
        }
    }

    public void StartNewGame(string sceneToLoad = "")
    {
        currentData = new PlayerData
        {
            currentHealth = 100f,
            maxHealth = 100f,
            spawnPosition = new Vector3(0, 2, 0)
        };

        SaveGame();

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(currentData, true);
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        File.WriteAllText(path, json);
        Debug.Log($"Game Saved to: {path}");
    }

    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Game Loaded");
        }
        else
        {
            StartNewGame();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentData.spawnPosition = position;
        SaveGame();
    }

    public void UpdateHealth(float amount)
    {
        currentData.currentHealth = amount;
    }

    public void RespawnPlayer(GameObject player)
    {
        if (player == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null) cc.enabled = false;

        player.transform.position = currentData.spawnPosition;
        player.transform.rotation = Quaternion.identity;

        if (cc != null) cc.enabled = true;

        currentData.currentHealth = currentData.maxHealth;

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null) ph.ResetHealth();
    }

    [ContextMenu("Delete Save File")]
    public void DeleteSave()
    {
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        if (File.Exists(path)) File.Delete(path);
        Debug.Log("Save File Deleted");
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        SaveGame();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

[System.Serializable]
public class PlayerData
{
    public float currentHealth;
    public float maxHealth;
    public Vector3 spawnPosition;

    public float musicVolume = 1f;
    public float sfxVolume = 1f;
}