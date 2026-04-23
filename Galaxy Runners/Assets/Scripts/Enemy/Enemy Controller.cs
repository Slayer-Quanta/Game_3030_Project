using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Border verticalSpeed;
    [SerializeField] private Border horizontalSpeed;
    [SerializeField] private Border verticalBounds;
    [SerializeField] private Border horizontalBounds;

    private float verticalspeed;
    private float horizontalspeed;
    private SpriteRenderer spriteRenderer;
    private Collider2D _collider;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        InitializeEnemy();
    }

    private void Update()
    {
        MoveEnemy();
        CheckOutOfBounds();
    }

    private void MoveEnemy()
    {
        transform.position = new Vector3(
            Mathf.PingPong(horizontalspeed * Time.time, horizontalBounds.Max - horizontalBounds.Min) + horizontalBounds.Min,
            transform.position.y - verticalspeed * Time.deltaTime,
            -1);
    }

    private void CheckOutOfBounds()
    {
        if (transform.position.y < verticalBounds.Min)
        {
            InitializeEnemy();
        }
    }

    public void TriggerDeathSequence()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        if (_collider != null) _collider.enabled = false;

        spriteRenderer.color = Color.red;

        if (ScoreManager.instance != null) ScoreManager.instance.AddScore(5);
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("EnemyDeath");

        yield return new WaitForSeconds(0.2f);

        InitializeEnemy();
    }

    private void InitializeEnemy()
    {
        spriteRenderer.enabled = true;
        spriteRenderer.color = Color.white;
        if (_collider != null) _collider.enabled = true;

        transform.position = new Vector3(
            Random.Range(horizontalBounds.Min, horizontalBounds.Max),
            verticalBounds.Max,
            -1
        );
        verticalspeed = Random.Range(verticalSpeed.Min, verticalSpeed.Max);
        horizontalspeed = Random.Range(horizontalSpeed.Min, horizontalSpeed.Max);
    }
}