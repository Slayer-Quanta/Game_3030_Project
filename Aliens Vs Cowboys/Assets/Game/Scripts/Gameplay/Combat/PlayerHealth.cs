using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100f;
    public float returnToMenuDelay = 3f;
    public string mainMenuSceneName = "Main Menu";

    [Header("Events")]
    public UnityEvent OnDeath;
    public UnityEvent OnDamaged;
    public UnityEvent OnHealthChanged;

    [Header("Current State")]
    [SerializeField] private float currentHealth;

    public float RemainingHealthPercentage => currentHealth / maxHealth;
    public bool isInvincible { get; set; }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.currentData.currentHealth;
            maxHealth = GameManager.Instance.currentData.maxHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0 || isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"Player Health: {currentHealth}");

        // if (AudioManager.instance != null) AudioManager.instance.PlaySFX("PlayerHurt");

        if (currentHealth <= 0) currentHealth = 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentData.currentHealth = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            OnDamaged?.Invoke();
        }

        OnHealthChanged?.Invoke();
    }

    public void AddHealth(float amountToAdd)
    {
        if (currentHealth >= maxHealth) return;

        currentHealth += amountToAdd;

        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentData.currentHealth = currentHealth;
        }

        OnHealthChanged?.Invoke();
    }

    void Die()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("PlayerDeath");

        OnDeath?.Invoke();
        Debug.Log("Player Died! Returning to menu in 3 seconds...");

        yield return new WaitForSeconds(returnToMenuDelay);

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentData.currentHealth = currentHealth;
        }

        OnHealthChanged?.Invoke();
    }
}