using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public float maxHealth = 30f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX("EnemyDeath");

            Destroy(gameObject);
        }
    }
}