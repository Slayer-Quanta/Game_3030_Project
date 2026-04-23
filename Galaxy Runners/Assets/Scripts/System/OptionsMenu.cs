using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OptionsMenu : MonoBehaviour
{
    [Header("References")]
    public GameSettings settings;
    private AudioManager audioManager;

    private const float VOLUME_STEP = 0.1f;

    [Header("Panels")]
    public GameObject categoryPanel;
    public GameObject audioPanel;

    public Action OnReturnToParentMenu;

    [Header("Audio UI")]
    public TMP_Text musicVolumeValueText;
    public Button musicVolumeIncreaseButton;
    public Button musicVolumeDecreaseButton;

    public TMP_Text sfxVolumeValueText;
    public Button sfxVolumeIncreaseButton;
    public Button sfxVolumeDecreaseButton;

    void Awake()
    {
        audioManager = AudioManager.instance;
    }

    public void Initialize()
    {
        if (settings == null)
        {
            Debug.LogError("GameSettings missing from OptionsMenu", this);
            return;
        }

        CloseAllPanels();
        ShowCategoryPanel();
        InitializeAudio();
    }

    private void CloseAllPanels()
    {
        categoryPanel?.SetActive(false);
        audioPanel?.SetActive(false);
    }

    public void ShowCategoryPanel()
    {
        CloseAllPanels();
        categoryPanel.SetActive(true);
        audioManager?.PlayButtonClick();
    }

    public void OpenAudioPanel()
    {
        CloseAllPanels();
        audioPanel.SetActive(true);
        audioManager?.PlayButtonClick();
    }

    public void GoBack()
    {
        audioManager?.PlayButtonClick();

        if (audioPanel.activeSelf)
        {
            ShowCategoryPanel();
        }
        else
        {
            OnReturnToParentMenu?.Invoke();
        }
    }

    private void InitializeAudio()
    {
        UpdateVolumeText(musicVolumeValueText, settings.musicVolume);
        musicVolumeIncreaseButton.onClick.RemoveAllListeners();
        musicVolumeDecreaseButton.onClick.RemoveAllListeners();
        musicVolumeIncreaseButton.onClick.AddListener(() => IncrementVolume(ref settings.musicVolume, musicVolumeValueText, OnMusicVolumeChanged));
        musicVolumeDecreaseButton.onClick.AddListener(() => DecrementVolume(ref settings.musicVolume, musicVolumeValueText, OnMusicVolumeChanged));

        UpdateVolumeText(sfxVolumeValueText, settings.sfxVolume);
        sfxVolumeIncreaseButton.onClick.RemoveAllListeners();
        sfxVolumeDecreaseButton.onClick.RemoveAllListeners();
        sfxVolumeIncreaseButton.onClick.AddListener(() => IncrementVolume(ref settings.sfxVolume, sfxVolumeValueText, OnSFXVolumeChanged));
        sfxVolumeDecreaseButton.onClick.AddListener(() => DecrementVolume(ref settings.sfxVolume, sfxVolumeValueText, OnSFXVolumeChanged));
    }

    public void IncrementVolume(ref float value, TMP_Text text, Action<float> callback)
    {
        value = Mathf.Min(value + VOLUME_STEP, 1f);
        callback(value);
    }

    public void DecrementVolume(ref float value, TMP_Text text, Action<float> callback)
    {
        value = Mathf.Max(value - VOLUME_STEP, 0f);
        callback(value);
    }

    public void OnMusicVolumeChanged(float volume)
    {
        settings.musicVolume = volume;
        audioManager?.SetMusicVolume(volume);
        UpdateVolumeText(musicVolumeValueText, volume);
        audioManager?.PlayButtonClick();
    }

    public void OnSFXVolumeChanged(float volume)
    {
        settings.sfxVolume = volume;
        audioManager?.SetSFXVolume(volume);
        UpdateVolumeText(sfxVolumeValueText, volume);
        audioManager?.PlayButtonClick();
    }

    private void UpdateVolumeText(TMP_Text text, float value)
    {
        text.text = $"Volume: {Mathf.RoundToInt(value * 100)}%";
    }

    private void OnDestroy()
    {
        musicVolumeIncreaseButton?.onClick.RemoveAllListeners();
        musicVolumeDecreaseButton?.onClick.RemoveAllListeners();
        sfxVolumeIncreaseButton?.onClick.RemoveAllListeners();
        sfxVolumeDecreaseButton?.onClick.RemoveAllListeners();
    }
}