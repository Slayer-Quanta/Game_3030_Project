using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System.Collections; 

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;

    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;

    private InputSystem_Actions _actions;
    private float2 movementInput;

    void Awake()
    {
        _actions = new InputSystem_Actions();
        if (cam == null) cam = Camera.main;
    }

    void OnEnable()
    {
        _actions.Player.Enable();
        _actions.Player.Dash.performed += OnDashPerformed;
    }

    void OnDisable()
    {
        _actions.Player.Dash.performed -= OnDashPerformed;
        _actions.Player.Disable();
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (PauseManager.IsGamePaused || isDashing || !canDash) return;

        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        Vector2 dashDir = math.length(movementInput) > 0 ? new Vector2(movementInput.x, movementInput.y).normalized : (Vector2)transform.up;

        rb.linearVelocity = dashDir * dashForce;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Update()
    {
        if (PauseManager.IsGamePaused) return;
        movementInput = _actions.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (PauseManager.IsGamePaused || isDashing) return;

        float3 currentPos = new float3(rb.position.x, rb.position.y, 0);
        float3 velocity = new float3(movementInput.x, movementInput.y, 0) * moveSpeed * Time.fixedDeltaTime;
        float3 newPos = currentPos + velocity;

        rb.MovePosition(new Vector2(newPos.x, newPos.y));

        // Rotation logic
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