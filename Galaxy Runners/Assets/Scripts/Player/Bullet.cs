using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isEnemyBullet = false;
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isEnemyBullet && collider.GetComponent<EnemyController>())
        {
            return;
        }

        if (!isEnemyBullet)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TriggerDeathSequence();
                Destroy(gameObject);
                return;
            }
        }

        if (isEnemyBullet && collider.CompareTag("Player"))
        {
            PlayerShip playerShip = collider.GetComponent<PlayerShip>();
            if (playerShip != null)
            {
                playerShip.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}