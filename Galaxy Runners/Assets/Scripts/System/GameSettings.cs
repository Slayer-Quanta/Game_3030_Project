using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Audio")]
    [Range(0f, 1f)]
    public float musicVolume = 0.75f;

    [Range(0f, 1f)]
    public float sfxVolume = 0.75f;
}