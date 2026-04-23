using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScroller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 3.0f;

    private float _backgroundHeight;
    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        _backgroundHeight = spriteRenderer.bounds.size.y;
    }

    void Update()
    {
        float newY = Mathf.Repeat(Time.time * speed, _backgroundHeight);
        transform.position = _startPosition + Vector3.down * newY;
    }
}