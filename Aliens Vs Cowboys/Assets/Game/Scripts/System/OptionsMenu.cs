using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{
    [Header("References")]
    public GameSettings settings;
    private AudioManager audioManager;

    private const float VOLUME_STEP = 0.1f;
    private const float SENSITIVITY_STEP = 0.25f;

    [Header("Panels")]
    public GameObject categoryPanel;
    public GameObject videoPanel;
    public GameObject audioPanel;
    public GameObject controlsPanel;

    public Action OnReturnToParentMenu;

    [Header("Controls UI")]
    public TMP_Text sensitivityValueText;
    public Button sensitivityIncreaseButton;
    public Button sensitivityDecreaseButton;

    [Header("Audio UI")]
    public TMP_Text musicVolumeValueText;
    public Button musicVolumeIncreaseButton;
    public Button musicVolumeDecreaseButton;

    public TMP_Text sfxVolumeValueText;
    public Button sfxVolumeIncreaseButton;
    public Button sfxVolumeDecreaseButton;

    [Header("Graphics UI")]
    public TMP_Dropdown graphicsDropdown;

    void Awake()
    {
        audioManager = AudioManager.instance;

    }

    public void Initialize()
    {
        if (settings == null)
        {
            Debug.LogError("GameSettings missing from OptionsMenu!", this);
            return;
        }

        if (categoryPanel == null)
        {
            Debug.LogError("Category Panel not assigned in OptionsMenu!", this);
            return;
        }

        CloseAllPanels();
        ShowCategoryPanel();

        InitializeSensitivity();
        InitializeAudio();
        InitializeGraphics();
    }
    private void CloseAllPanels()
    {
        categoryPanel?.SetActive(false);
        videoPanel?.SetActive(false);
        audioPanel?.SetActive(false);
        controlsPanel?.SetActive(false);
    }

    public void ShowCategoryPanel()
    {
        CloseAllPanels();
        categoryPanel.SetActive(true);
        audioManager?.PlayButtonClick();
    }

    public void OpenVideoPanel()
    {
        CloseAllPanels();
        videoPanel.SetActive(true);
        audioManager?.PlayButtonClick();
    }

    public void OpenAudioPanel()
    {
        CloseAllPanels();
        audioPanel.SetActive(true);
        audioManager?.PlayButtonClick();
    }

    public void OpenControlsPanel()
    {
        CloseAllPanels();
        controlsPanel.SetActive(true);
        audioManager?.PlayButtonClick();
    }

    public void GoBack()
    {
        audioManager?.PlayButtonClick();

        if (videoPanel.activeSelf || audioPanel.activeSelf || controlsPanel.activeSelf)
        {
            ShowCategoryPanel();
        }
        else
        {
            OnReturnToParentMenu?.Invoke();
        }
    }

    private void InitializeSensitivity()
    {
        var range = (RangeAttribute)Attribute.GetCustomAttribute(
            typeof(GameSettings).GetField("mouseSensitivity"),
            typeof(RangeAttribute)
        );

        float min = range?.min ?? 0.1f;
        float max = range?.max ?? 5f;

        UpdateSensitivityText(settings.mouseSensitivity);

        sensitivityIncreaseButton.onClick.RemoveAllListeners();
        sensitivityDecreaseButton.onClick.RemoveAllListeners();

        sensitivityIncreaseButton.onClick.AddListener(() => IncrementSensitivity(max));
        sensitivityDecreaseButton.onClick.AddListener(() => DecrementSensitivity(min));
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

    private void InitializeGraphics()
    {
        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(new List<string>(QualitySettings.names));
        graphicsDropdown.value = settings.graphicsQualityIndex;

        graphicsDropdown.onValueChanged.RemoveAllListeners();
        graphicsDropdown.onValueChanged.AddListener(OnGraphicsChanged);

        ApplyGraphics(settings.graphicsQualityIndex);
    }

    public void IncrementSensitivity(float max)
    {
        settings.mouseSensitivity = Mathf.Min(settings.mouseSensitivity + SENSITIVITY_STEP, max);
        OnSensitivityChanged(settings.mouseSensitivity);
    }

    public void DecrementSensitivity(float min)
    {
        settings.mouseSensitivity = Mathf.Max(settings.mouseSensitivity - SENSITIVITY_STEP, min);
        OnSensitivityChanged(settings.mouseSensitivity);
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

    public void OnSensitivityChanged(float value)
    {
        UpdateSensitivityText(value);
        audioManager?.PlayButtonClick();
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
    public void OnGraphicsChanged(int index)
    {
        settings.graphicsQualityIndex = index;
        ApplyGraphics(index);
        audioManager?.PlayButtonClick();
    }

    private void ApplyGraphics(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
    }


    private void UpdateSensitivityText(float value)
    {
        sensitivityValueText.text = $"Sensitivity: {value:F2}";
    }

    private void UpdateVolumeText(TMP_Text text, float value)
    {
        text.text = $"Volume: {Mathf.RoundToInt(value * 100)}%";
    }

    private void OnDestroy()
    {
        graphicsDropdown?.onValueChanged.RemoveAllListeners();
        sensitivityIncreaseButton?.onClick.RemoveAllListeners();
        sensitivityDecreaseButton?.onClick.RemoveAllListeners();
        musicVolumeIncreaseButton?.onClick.RemoveAllListeners();
        musicVolumeDecreaseButton?.onClick.RemoveAllListeners();
        sfxVolumeIncreaseButton?.onClick.RemoveAllListeners();
        sfxVolumeDecreaseButton?.onClick.RemoveAllListeners();
    }
}