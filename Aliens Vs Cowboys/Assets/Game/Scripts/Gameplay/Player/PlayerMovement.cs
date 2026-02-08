using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;

    private InputSystem_Actions _actions;
    private float2 _moveInput;

    void Awake()
    {
        _actions = new InputSystem_Actions();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void OnEnable()
    {
        _actions.Player.Enable();
    }

    void OnDisable()
    {
        _actions.Player.Disable();
    }

    void Update()
    {
        _moveInput = _actions.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        float3 currentPos = new float3(rb.position.x, rb.position.y, 0);

        float3 velocity = new float3(_moveInput.x, _moveInput.y, 0) * moveSpeed * Time.fixedDeltaTime;
        float3 newPos = currentPos + velocity;

        rb.MovePosition(new Vector2(newPos.x, newPos.y));
        float2 mouseScreenPos = Mouse.current.position.ReadValue();
        float3 mouseWorldPos = (float3)cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, cam.nearClipPlane));

        float3 lookDir = mouseWorldPos - currentPos;

        float angle = math.degrees(math.atan2(lookDir.y, lookDir.x)) - 90f;

        rb.rotation = angle;

    }

    void LateUpdate()
    {
        if (cam != null)
        {
            float3 playerPos = (float3)transform.position;
            float3 camPos = new float3(playerPos.x, playerPos.y, -10f);
            cam.transform.position = (Vector3)camPos;
        }
    }
}