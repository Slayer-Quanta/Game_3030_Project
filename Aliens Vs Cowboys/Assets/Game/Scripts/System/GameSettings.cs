using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Mouse Look")]
    [Range(0.1f, 10f)]
    public float mouseSensitivity = 2f;

    [Header("Audio")]
    [Range(0f, 1f)]
    public float musicVolume = 0.75f;

    [Range(0f, 1f)]
    public float sfxVolume = 0.75f;

    [Header("Graphics")]
    [Range(0, 5)]
    public int graphicsQualityIndex = 3;
}