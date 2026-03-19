using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Weapon Settings")]
    public FireMode currentWeapon = FireMode.Single;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 8f;

    [Header("Fire Rates")]
    [SerializeField] private float singleFireRate = 0.5f;
    [SerializeField] private float sprayFireRate = 0.1f;
    [SerializeField] private float shotgunFireRate = 0.8f;

    [Header("Gun Offsets")]
    [SerializeField] private Transform gunOffset;
    [SerializeField] private Transform gunOffsetLeft;
    [SerializeField] private Transform gunOffsetRight;

    [Header("Animation")]
    public Animator animator;

    [Header("Camera")]
    public Camera mainCamera;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    private Vector2 smoothMovementInput;
    private Vector2 movementInputSmoothVelocity;

    private float lastFireTime;

    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction switchAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        fireAction = playerInput.actions.FindAction("Fire");
        switchAction = playerInput.actions.FindAction("Switch");
    }

    private void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }

    void Update()
    {
        if (switchAction != null && switchAction.WasPressedThisFrame())
        {
            CycleWeapon();
        }

        bool isShootingInput = fireAction != null && fireAction.IsPressed();
        bool isMovingInput = movementInput != Vector2.zero;

        if (isShootingInput)
        {
            float timeSinceLastFire = Time.time - lastFireTime;
            float currentFireRate = GetCurrentFireRate();

            if (timeSinceLastFire >= currentFireRate)
            {
                FireWeapon();
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

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            Vector3 targetCamPos = new Vector3(transform.position.x, transform.position.y, -10f);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, 10f * Time.deltaTime);
        }
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

    private void CycleWeapon()
    {
        int nextWeaponIndex = ((int)currentWeapon + 1) % System.Enum.GetValues(typeof(FireMode)).Length;
        currentWeapon = (FireMode)nextWeaponIndex;

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Swap");
    }

    private float GetCurrentFireRate()
    {
        switch (currentWeapon)
        {
            case FireMode.Spray: return sprayFireRate;
            case FireMode.Shotgun: return shotgunFireRate;
            default: return singleFireRate;
        }
    }

    private void FireWeapon()
    {
        switch (currentWeapon)
        {
            case FireMode.Single:
                SpawnBullet(gunOffset);
                if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Single");
                break;

            case FireMode.Spray:
                SpawnBullet(gunOffset);
                if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Spray");
                break;

            case FireMode.Shotgun:
                SpawnBullet(gunOffset);
                SpawnBullet(gunOffsetLeft);
                SpawnBullet(gunOffsetRight);
                if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Shotgun");
                break;
        }
    }

    private void SpawnBullet(Transform spawnPoint)
    {
        if (spawnPoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody2D rigidBody = bullet.GetComponent<Rigidbody2D>();

        if (rigidBody != null)
        {
            rigidBody.linearVelocity = bulletSpeed * spawnPoint.up;
        }
    }
}