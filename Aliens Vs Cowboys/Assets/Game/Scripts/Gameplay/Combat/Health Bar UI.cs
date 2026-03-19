using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthBarForegroundImage;


    public void UpdateHealthBar(PlayerHealth playerHealth)
    {
        healthBarForegroundImage.fillAmount = playerHealth.RemainingHealthPercentage;
    }
}