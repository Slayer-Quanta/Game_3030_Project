using UnityEngine;
using Unity.Mathematics;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float stopDistance = 5f;
    public float retreatDistance = 3f;

    [Header("Combat Settings")]
    public float shootingRange = 10f;
    public float fireRate = 1.2f;
    private float nextFireTime;

    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 12f;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float3 myPos = transform.position;
        float3 playerPos = player.position;

        float distanceToPlayer = math.distance(myPos, playerPos);
        float3 targetPos = myPos; 

        if (distanceToPlayer > stopDistance)
        {
            targetPos = MoveTowards(myPos, playerPos, moveSpeed * Time.deltaTime);
        }
        else if (distanceToPlayer < retreatDistance)
        {
            float3 dirAway = math.normalize(myPos - playerPos);
            float3 retreatPos = myPos + dirAway;
            targetPos = MoveTowards(myPos, retreatPos, moveSpeed * Time.deltaTime);
        }

        transform.position = (Vector3)targetPos;

        float3 lookDir = playerPos - myPos;
        float angle = math.degrees(math.atan2(lookDir.y, lookDir.x)) - 90f;

        transform.rotation = quaternion.Euler(0, 0, math.radians(angle));

        if (distanceToPlayer < shootingRange && Time.time > nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + fireRate;
        }
    }

    float3 MoveTowards(float3 current, float3 target, float maxDistDelta)
    {
        float3 dir = target - current;
        float dist = math.length(dir);

        if (dist <= maxDistDelta || dist == 0f)
        {
            return target;
        }

        return current + (dir / dist) * maxDistDelta;
    }

    void ShootAtPlayer()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            float3 fireUp = (float3)firePoint.up;
            float3 force = fireUp * bulletForce;

            rb.AddForce(new Vector2(force.x, force.y), ForceMode2D.Impulse);
        }
    }
}