using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Shooting Settings (From Video)")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private Transform gunOffset;
    [SerializeField] private float timeBetweenShots = 0.5f;

    [Header("Animation")]
    public Animator animator; 

    private Rigidbody2D rb;
    private Vector2 movementInput;

    private Vector2 smoothMovementInput;
    private Vector2 movementInputSmoothVelocity;

    private float lastFireTime;

    private PlayerInput playerInput;
    private InputAction fireAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        fireAction = playerInput.actions["Fire"];
    }

    private void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }

    void Update()
    {
        bool isShootingInput = fireAction.IsPressed();
        bool isMovingInput = movementInput != Vector2.zero;

        if (isShootingInput)
        {
            float timeSinceLastFire = Time.time - lastFireTime;

            if (timeSinceLastFire >= timeBetweenShots)
            {
                FireBullet();
                lastFireTime = Time.time;
            }
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", isMovingInput);
            animator.SetBool("isIdle", !isMovingInput && !isShootingInput);
            animator.SetBool("isShooting", isShootingInput);
        }
    }

    void FixedUpdate()
    {
        smoothMovementInput = Vector2.SmoothDamp(
            smoothMovementInput,
            movementInput,
            ref movementInputSmoothVelocity,
            0.1f);

        rb.linearVelocity = smoothMovementInput * speed;

        RotateInDirectionOfInput();
    }

    private void RotateInDirectionOfInput()
    {
        if (movementInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, smoothMovementInput);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rb.MoveRotation(rotation);
        }
    }

    private void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunOffset.position, transform.rotation);
        Rigidbody2D rigidBody = bullet.GetComponent<Rigidbody2D>();

        rigidBody.linearVelocity = bulletSpeed * transform.up;
    }
}