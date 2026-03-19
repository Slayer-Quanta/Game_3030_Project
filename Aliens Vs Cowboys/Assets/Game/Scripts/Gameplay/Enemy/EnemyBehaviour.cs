using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Movement & AI Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 200f;
    public float awarenessDistance = 8f; 

    [Header("Combat Spacing Settings")]
    public float stopDistance = 5f;
    public float retreatDistance = 3f;

    [Header("Combat Settings")]
    public float shootingRange = 10f;
    public float fireRate = 1.2f;
    private float nextFireTime;

    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 12f;

    private Rigidbody2D rb;
    private Transform player;

    private Vector2 targetDirection;
    private float directionChangeCooldown;
    private bool isStopped = false; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        targetDirection = transform.up;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < shootingRange && Time.time > nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        HandleRandomDirectionChange();

        HandlePlayerTargeting();
    }

    private void HandleRandomDirectionChange()
    {
        directionChangeCooldown -= Time.deltaTime;

        if (directionChangeCooldown <= 0f)
        {
            float randomAngle = Random.Range(-90f, 90f);

            Quaternion rotation = Quaternion.AngleAxis(randomAngle, transform.forward);
            targetDirection = rotation * targetDirection;

            directionChangeCooldown = Random.Range(1f, 5f);
        }
    }

    private void HandlePlayerTargeting()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= awarenessDistance)
        {
            if (distance > stopDistance)
            {
                targetDirection = (player.position - transform.position).normalized;
                isStopped = false;
            }
            else if (distance < retreatDistance)
            {
                targetDirection = (transform.position - player.position).normalized;
                isStopped = false;
            }
            else
            {
                targetDirection = (player.position - transform.position).normalized;
                isStopped = true;
            }
        }
        else
        {
            isStopped = false;
        }
    }

    private void RotateTowardsTarget()
    {
        if (targetDirection == Vector2.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDirection);

        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        rb.MoveRotation(rotation);
    }

    private void SetVelocity()
    {
        if (isStopped)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = transform.up * moveSpeed;
        }
    }

    void ShootAtPlayer()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.isEnemyBullet = true;
            }

            Rigidbody2D projRb = bulletObj.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                projRb.linearVelocity = firePoint.up * bulletForce;
            }
        }
    }
}