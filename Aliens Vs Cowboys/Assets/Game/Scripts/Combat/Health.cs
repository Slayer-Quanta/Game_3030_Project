using UnityEngine;
using UnityEngine.SceneManagement; 

public class Health : MonoBehaviour
{
    public float health = 3f;
    public bool isPlayer = false;

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isPlayer)
        {
            // Loads the main menu when I die, only for proof of concept
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            // Destroy the enemy 
            Destroy(gameObject);
        }
    }
}