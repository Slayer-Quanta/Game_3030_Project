using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            PlayerHealth healthController = player.GetComponent<PlayerHealth>();
            if (healthController != null)
            {
                healthController.TakeDamage(damageAmount);
            }
        }
    }
}