using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float movementSmoothing = 0.05f;

    [Header("Looking/Facing Settings")]
    [Tooltip("Offset (in degrees) if your sprite faces backwards/sideways. Default 'Right' is 0.")]
    public float spriteRotationOffset = 0f;

    [Header("Weapon Settings")]
    public FireMode currentWeapon = FireMode.Single;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 8f;

    [Header("Fire Rates")]
    [SerializeField] private float singleFireRate = 0.3f;
    [SerializeField] private float sprayFireRate = 0.1f;
    [SerializeField] private float shotgunFireRate = 0.6f;

    [Header("Spawn Points")]
    [SerializeField] private Transform gunOffset;
    [SerializeField] private Transform gunOffsetLeft;
    [SerializeField] private Transform gunOffsetRight;

    [Header("Required Components")]
    public Animator animator;
    public Camera mainCamera;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction fireAction;
    private InputAction switchAction;

    private Vector2 rawMovementInput;
    private Vector2 smoothMovementInput;
    private Vector2 movementInputSmoothVelocity;

    private float targetAngle;
    private float lastFireTime;

    public enum FireMode { Single, Spray, Shotgun }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        rb.gravityScale = 0f;
        rb.angularDamping = 0f;

        if (mainCamera == null) mainCamera = Camera.main;

        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        fireAction = playerInput.actions.FindAction("Fire");
        switchAction = playerInput.actions.FindAction("Switch");
    }

    void Update()
    {
        HandleInput();
        HandleShooting();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        ApplyMovementAndRotation();
    }

    private void HandleInput()
    {
        // 1. Read Movement
        if (moveAction != null)
        {
            rawMovementInput = moveAction.ReadValue<Vector2>();
        }

        // 2. Read Aiming (Safely separated by Control Scheme!)
        if (mainCamera != null && playerInput != null)
        {
            string currentScheme = playerInput.currentControlScheme;

            // If we are using a Controller, ONLY read the stick
            if (currentScheme == "Gamepad" || currentScheme == "Joystick")
            {
                if (lookAction != null)
                {
                    Vector2 stickInput = lookAction.ReadValue<Vector2>();

                    // Only update the angle if they are actually pushing the stick
                    if (stickInput.sqrMagnitude > 0.01f)
                    {
                        CalculateGamepadRotation(stickInput);
                    }
                }
            }
            // If we are using Keyboard/Mouse, ONLY read the hardware mouse
            else
            {
                if (Mouse.current != null)
                {
                    CalculateMouseRotation(Mouse.current.position.ReadValue());
                }
            }
        }

        // 3. Cycle Weapon
        if (switchAction != null && switchAction.WasPressedThisFrame()) CycleWeapon();
    }

    private void CalculateGamepadRotation(Vector2 stickInput)
    {
        targetAngle = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;
    }

    private void CalculateMouseRotation(Vector2 mouseScreenPos)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
        Vector2 aimDir = (Vector2)(mouseWorldPos - transform.position);

        if (aimDir.sqrMagnitude > 0.01f)
        {
            targetAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        }
    }

    private void HandleShooting()
    {
        bool isShootingInput = fireAction != null && fireAction.IsPressed();

        if (isShootingInput)
        {
            float timeSinceLastFire = Time.time - lastFireTime;
            if (timeSinceLastFire >= GetCurrentFireRate())
            {
                FireWeapon();
                lastFireTime = Time.time;
            }
        }
    }

    private void FireWeapon()
    {
        switch (currentWeapon)
        {
            case FireMode.Single:
                SpawnBullet(gunOffset);
                break;
            case FireMode.Spray:
                SpawnBullet(gunOffset);
                break;
            case FireMode.Shotgun:
                SpawnBullet(gunOffset);
                SpawnBullet(gunOffsetLeft);
                SpawnBullet(gunOffsetRight);
                break;
        }
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

    private void SpawnBullet(Transform spawnPoint)
    {
        if (spawnPoint == null || bulletPrefab == null) return;
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null) bulletRb.linearVelocity = spawnPoint.up * bulletSpeed;
    }

    private void CycleWeapon()
    {
        int totalWeapons = System.Enum.GetValues(typeof(FireMode)).Length;
        int nextWeaponIndex = ((int)currentWeapon + 1) % totalWeapons;
        currentWeapon = (FireMode)nextWeaponIndex;
    }

    private void ApplyMovementAndRotation()
    {
        // Smoothly apply velocity instead of MovePosition to prevent physics jittering
        smoothMovementInput = Vector2.SmoothDamp(
            smoothMovementInput,
            rawMovementInput,
            ref movementInputSmoothVelocity,
            movementSmoothing);

        rb.linearVelocity = smoothMovementInput * speed;

        // Apply our isolated aiming angle
        rb.MoveRotation(targetAngle + spriteRotationOffset);
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            bool isMovingInput = rawMovementInput.sqrMagnitude > 0.01f;
            bool isShootingInput = fireAction != null && fireAction.IsPressed();

            animator.SetBool("isWalking", isMovingInput);
            animator.SetBool("isIdle", !isMovingInput && !isShootingInput);
            animator.SetBool("isShooting", isShootingInput);
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            Vector3 targetCamPos = new Vector3(transform.position.x, transform.position.y, -10f);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, 8f * Time.deltaTime);
        }
    }
}