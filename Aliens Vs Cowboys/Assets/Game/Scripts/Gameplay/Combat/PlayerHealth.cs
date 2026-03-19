using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100f;

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
        // Don't take damage if dead or currently invincible
        if (currentHealth <= 0 || isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"Player Health: {currentHealth}");

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

        // Notify UI that health changed
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

        // Notify UI that health changed
        OnHealthChanged?.Invoke();
    }

    void Die()
    {
        OnDeath?.Invoke();
        Debug.Log("Player Died!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RespawnPlayer(this.gameObject);

            currentHealth = GameManager.Instance.currentData.currentHealth;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentData.currentHealth = currentHealth;
        }

        // Update UI when resetting health
        OnHealthChanged?.Invoke();
    }
}