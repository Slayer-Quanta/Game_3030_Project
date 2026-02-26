using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System;

public partial class PlayerBehaviour : MonoBehaviour
{
    [Header("Weapon Settings")]
    public FireMode currentMode = FireMode.Single;
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("General Stats")]
    public float bulletForce = 20f;
    public float sprayFireRate = 0.1f;
    private float _nextShotTime = 0f;
    private bool _isFiring = false;

    [Header("Shotgun Settings")]
    public int shotgunPelletCount = 5;
    public float spreadAngle = 15f;

    private InputSystem_Actions _actions;

    private Action<InputAction.CallbackContext> _onAttackStarted;
    private Action<InputAction.CallbackContext> _onAttackCanceled;
    private Action<InputAction.CallbackContext> _onSwitchPerformed;

    void Awake() => _actions = new InputSystem_Actions();

    void OnEnable()
    {
        _actions.Player.Enable();

        _onAttackStarted = ctx => _isFiring = true;
        _onAttackCanceled = ctx => _isFiring = false;
        _onSwitchPerformed = ctx => CycleWeapon();

        _actions.Player.Attack.started += _onAttackStarted;
        _actions.Player.Attack.canceled += _onAttackCanceled;
        _actions.Player.Attack.performed += OnAttackPerformed;
        _actions.Player.Switch.performed += _onSwitchPerformed;
    }

    void OnDisable()
    {
        _actions.Player.Attack.started -= _onAttackStarted;
        _actions.Player.Attack.canceled -= _onAttackCanceled;
        _actions.Player.Attack.performed -= OnAttackPerformed;
        _actions.Player.Switch.performed -= _onSwitchPerformed;
        _actions.Player.Disable();
    }

    private void CycleWeapon()
    {
        FireMode[] modes = (FireMode[])System.Enum.GetValues(typeof(FireMode));
        int nextIndex = ((int)currentMode + 1) % modes.Length;
        currentMode = modes[nextIndex];
        Debug.Log($"Switched to: {currentMode}");
    }

    void Update()
    {
        if (_isFiring && currentMode == FireMode.Spray && Time.time >= _nextShotTime)
        {
            ShootSingle();
            _nextShotTime = Time.time + sprayFireRate;
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (PauseManager.IsGamePaused) return;

        if (currentMode == FireMode.Single) ShootSingle();
        else if (currentMode == FireMode.Shotgun) ShootShotgun();
    }

    void ShootSingle()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        ApplyBulletForce(bullet, firePoint.up);
    }

    void ShootShotgun()
    {
        for (int i = 0; i < shotgunPelletCount; i++)
        {
            float addedOffset = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
            Quaternion spreadRotation = firePoint.rotation * Quaternion.Euler(0, 0, addedOffset);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, spreadRotation);
            ApplyBulletForce(bullet, bullet.transform.up);
        }
    }

    void ApplyBulletForce(GameObject bullet, Vector3 direction)
    {
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null) bulletScript.isEnemyBullet = false;
    }
}