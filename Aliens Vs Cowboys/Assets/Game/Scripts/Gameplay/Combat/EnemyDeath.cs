using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public float health = 3f;

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
        Destroy(gameObject);
    }
}