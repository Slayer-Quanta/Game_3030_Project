using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;
    public float damage = 1f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Health targetHealth = collision.gameObject.GetComponent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 5f);
        }

        Destroy(gameObject);
    }
}