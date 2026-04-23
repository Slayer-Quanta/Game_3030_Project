using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Border
{
    public float Min, Max;
}

[RequireComponent(typeof(PlayerInput))]
public class PlayerShip : MonoBehaviour
{
    [Header("Scene Management")]
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    [Header("Movement & Bounds")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 250f;
    [SerializeField] private Border horizontalBounds;
    [SerializeField] private Border verticalBounds;

    [Header("Combat Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float bulletSpeed = 15f;

    private SpriteRenderer _spriteRenderer;
    private PlayerInput _playerInput;

    private Vector2 _moveInput;
    private float _targetAngle;
    private int _hitCount = 0;
    private float _nextFireTime;
    private float _zPosition = -1f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _targetAngle = transform.eulerAngles.z;
        StartCoroutine(ScoreIncrementCoroutine());
    }

    private void Update()
    {
        if (PauseManager.IsGamePaused || Time.timeScale == 0) return;

        HandleInputs();
        ApplyMovementAndWrapping();

        if (_playerInput.actions["Fire"].IsPressed() && Time.time > _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + fireRate;
        }
    }

    private void HandleInputs()
    {
        _moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();

        float turnLeft = _playerInput.actions["Rotate Left"].ReadValue<float>();
        float turnRight = _playerInput.actions["Rotate Right"].ReadValue<float>();
        float turnAmount = turnLeft - turnRight;

        _targetAngle += turnAmount * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, _targetAngle);
    }

    private void ApplyMovementAndWrapping()
    {
        transform.position += (Vector3)_moveInput * speed * Time.deltaTime;

        Vector3 pos = transform.position;
        if (pos.x > horizontalBounds.Max) pos.x = horizontalBounds.Min;
        else if (pos.x < horizontalBounds.Min) pos.x = horizontalBounds.Max;

        if (pos.y > verticalBounds.Max) pos.y = verticalBounds.Max;
        else if (pos.y < verticalBounds.Min) pos.y = verticalBounds.Min;

        pos.z = _zPosition;
        transform.position = pos;
    }

    private void Fire()
    {
        if (bulletPrefab == null || firePoint == null) return;
        GameObject b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        b.GetComponent<Rigidbody2D>().linearVelocity = firePoint.up * bulletSpeed;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX("Shoot");
        }
    }

    public void TakeDamage(float damageAmount = 1f)
    {
        _hitCount++;
        if (_hitCount == 1)
        {
            _spriteRenderer.color = Color.red;

            if (AudioManager.instance != null) AudioManager.instance.PlaySFX("PlayerHurt");
        }
        else if (_hitCount >= 2)
        {
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX("PlayerDeath");

            if (GameManager.Instance != null && ScoreManager.instance != null && ScoreManager.instance.scoreText != null)
            {
                string scoreString = ScoreManager.instance.scoreText.text.Replace("Score: ", "");
                if (int.TryParse(scoreString, out int finalScore))
                {
                    GameManager.Instance.CheckAndSaveHighScore(finalScore);
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Camera.main != null && Camera.main.transform.parent == transform)
            {
                Camera.main.transform.SetParent(null);
            }

            Destroy(gameObject);
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    private IEnumerator ScoreIncrementCoroutine()
    {
        while (true)
        {
            if (_hitCount < 2 && ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(1);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}