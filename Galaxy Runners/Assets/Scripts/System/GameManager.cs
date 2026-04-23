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
        if (scene.name == "MainMenu" || scene.name == "Main Menu") return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            RespawnPlayer(player);
        }
    }

    public void StartNewGame(string sceneToLoad = "")
    {
        if (currentData == null)
        {
            currentData = new PlayerData();
        }

        currentData.spawnPosition = new Vector3(0, -3.5f, 0);

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

    public void CheckAndSaveHighScore(int currentScore)
    {
        if (currentScore > currentData.highScore)
        {
            currentData.highScore = currentScore;
            SaveGame();
            Debug.Log($"New High Score: {currentData.highScore}!");
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentData.spawnPosition = position;
        SaveGame();
    }

    public void RespawnPlayer(GameObject player)
    {
        if (player == null) return;

        player.transform.position = currentData.spawnPosition;
        player.transform.rotation = Quaternion.identity;
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
    public int highScore = 0;
    public Vector3 spawnPosition;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
}