using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
}

[System.Serializable]
public class SceneMusic
{
    public string sceneName;
    public string musicName;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Sounds")]
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    [Header("Scene Music Settings")]
    public SceneMusic[] sceneMusicMap;
    public string defaultMusic = "Menu Background Music";
    public bool playMusicOnUnknownScenes = true;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeVolume();
        SceneManager.sceneLoaded += OnSceneLoaded;
        HandleSceneMusic(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeVolume()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentData != null)
        {
            SetMusicVolume(GameManager.Instance.currentData.musicVolume);
            SetSFXVolume(GameManager.Instance.currentData.sfxVolume);
        }
        else
        {
            SetMusicVolume(1f);
            SetSFXVolume(1f);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneMusic(scene.name);
    }

    private void HandleSceneMusic(string sceneName)
    {
        SceneMusic sceneMusic = Array.Find(sceneMusicMap, sm => sm.sceneName == sceneName);

        if (sceneMusic != null && !string.IsNullOrEmpty(sceneMusic.musicName))
        {
            if (musicSource.clip == null || musicSource.clip.name != sceneMusic.musicName)
                PlayMusic(sceneMusic.musicName);
        }
        else if (playMusicOnUnknownScenes && !string.IsNullOrEmpty(defaultMusic))
        {
            Sound s = Array.Find(musicSounds, sound => sound.name == defaultMusic);
            if (s != null && musicSource.clip != s.clip)
                PlayMusic(defaultMusic);
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        musicSource.clip = s.clip;

        musicSource.volume = musicSource.volume * s.volume;

        musicSource.pitch = s.pitch;
        musicSource.loop = s.loop;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"SFX: {name} not found!");
            return;
        }

        sfxSource.pitch = s.pitch;
        sfxSource.PlayOneShot(s.clip, sfxSource.volume * s.volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxSource.volume * volume);
    }

    public void SetMusicVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        musicSource.volume = clampedVolume;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentData.musicVolume = clampedVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        sfxSource.volume = clampedVolume;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentData.sfxVolume = clampedVolume;
        }
    }

    public void PlayButtonClick() => PlaySFX("Button Click");
    public void PlayJumpSound() => PlaySFX("Jump");
    public void PlayWalkSound() => PlaySFX("Walk");
    public void PlayAttackSound() => PlaySFX("Attack");
    public void PlayPickupSound() => PlaySFX("Pickup");
    public void PlayDashSound() => PlaySFX("Dash");
    public void PlayLandingSound() => PlaySFX("Landing");

    public void AddSceneMusic(string sceneName, string musicName)
    {
        SceneMusic existing = Array.Find(sceneMusicMap, sm => sm.sceneName == sceneName);
        if (existing != null) existing.musicName = musicName;
    }

    public void SaveVolumeSettings()
    {
        if (GameManager.Instance != null) GameManager.Instance.SaveGame();
    }
}