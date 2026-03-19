using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isEnemyBullet = false;
    public float damage = 10f; 

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isEnemyBullet && collider.GetComponent<EnemyDeath>())
        {
            return;
        }

        if (collider.GetComponent<EnemyDeath>())
        {
            Destroy(collider.gameObject);
            Destroy(gameObject);
        }

        if (isEnemyBullet && collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}