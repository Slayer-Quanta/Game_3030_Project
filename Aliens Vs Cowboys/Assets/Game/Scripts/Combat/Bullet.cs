using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;
    public float damage = 1f;

    public bool isEnemyBullet = true;

    void OnCollisionEnter2D(Collision2D collision)
    {
            if (isEnemyBullet && collision.gameObject.CompareTag("Enemy")) return;
            if (!isEnemyBullet && collision.gameObject.CompareTag("Player")) return;

            // Check if we hit an enemy
            EnemyDeath enemyHealth = collision.gameObject.GetComponent<EnemyDeath>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Check if we hit the player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            if (hitEffect != null)
            {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 5f);
        }

        Destroy(gameObject);
    }
}