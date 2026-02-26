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

        Vector2 dashDir = math.length(movementInput) > 0
            ? new Vector2(movementInput.x, movementInput.y).normalized
            : (Vector2)transform.up;

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
        float3 nextPos = currentPos + velocity;
        rb.MovePosition(new Vector2(nextPos.x, nextPos.y));

        if (cam != null)
        {
            Vector2 mouseScreen = _actions.Player.Look.ReadValue<Vector2>();
            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, cam.nearClipPlane));

            Vector2 lookDir = (Vector2)worldPos - rb.position;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
                rb.MoveRotation(angle);
            }
        }
    }

    void LateUpdate()
    {
        if (cam == null) return;

        float3 playerPos = (float3)transform.position;
        float3 targetCamPos = new float3(playerPos.x, playerPos.y, -10f);
        cam.transform.position = Vector3.Lerp(cam.transform.position, (Vector3)targetCamPos, 10f * Time.deltaTime);
    }
}