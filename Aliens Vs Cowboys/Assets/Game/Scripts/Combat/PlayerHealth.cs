using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100f;

    [Header("Events")]
    public UnityEvent OnDeath;

    [Header("Current State")]
    [SerializeField] private float currentHealth;

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
        currentHealth -= damage;
        Debug.Log($"Player Health: {currentHealth}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentData.currentHealth = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
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
    }
}